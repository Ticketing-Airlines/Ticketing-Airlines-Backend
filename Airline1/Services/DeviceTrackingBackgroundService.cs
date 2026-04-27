using Airline1.Data;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter;
using System.Text;
using System.Text.Json;

namespace Airline1.Services
{
    /// <summary>
    /// Background service that listens to MQTT topics registered in the TrackingDevice table
    /// and saves incoming GPS coordinates to the DeviceLocation history table.
    /// </summary>
    public class DeviceTrackingBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeviceTrackingBackgroundService> _logger;
        private readonly IConfiguration _configuration;

        private IMqttClient? _mqttClient;
        private List<TrackingDevice> _trackedDevices = [];

        public DeviceTrackingBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<DeviceTrackingBackgroundService> logger,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var brokerHost = _configuration["Mqtt:BrokerHost"] ?? "localhost";
            var brokerPort = _configuration.GetValue<int>("Mqtt:BrokerPort", 1883);
            var useWebSocket = _configuration.GetValue<bool>("Mqtt:UseWebSocket", false);

            // Auto-detect WebSocket for common HiveMQ public broker ports
            if (!useWebSocket && (brokerPort == 8000 || brokerPort == 8884))
            {
                useWebSocket = true;
            }

            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                _logger.LogInformation("MQTT message received on topic '{Topic}'", topic);

                // Find which device owns this topic
                var device = _trackedDevices.FirstOrDefault(d => d.MqttTopic == topic);
                if (device == null) return;

                // Parse payload
                double? lat = null;
                double? lon = null;

                try
                {
                    using var doc = JsonDocument.Parse(payload);
                    if (doc.RootElement.TryGetProperty("latitude", out var latEl) && latEl.TryGetDouble(out var latVal))
                        lat = latVal;
                    if (doc.RootElement.TryGetProperty("longitude", out var lonEl) && lonEl.TryGetDouble(out var lonVal))
                        lon = lonVal;

                    // Fallback property names
                    if (!lat.HasValue && doc.RootElement.TryGetProperty("lat", out var latAlt) && latAlt.TryGetDouble(out var latAltVal))
                        lat = latAltVal;
                    if (!lon.HasValue && doc.RootElement.TryGetProperty("lon", out var lonAlt) && lonAlt.TryGetDouble(out var lonAltVal))
                        lon = lonAltVal;
                    if (!lon.HasValue && doc.RootElement.TryGetProperty("lng", out var lngAlt) && lngAlt.TryGetDouble(out var lngAltVal))
                        lon = lngAltVal;
                }
                catch (JsonException)
                {
                    // Try "lat,lon" plain text format
                    var parts = payload.Split(',', StringSplitOptions.TrimEntries);
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var latParsed) &&
                        double.TryParse(parts[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var lonParsed))
                    {
                        lat = latParsed;
                        lon = lonParsed;
                    }
                }

                if (!lat.HasValue || !lon.HasValue)
                {
                    _logger.LogWarning("Could not parse GPS payload from device {DeviceId}. Payload: {Payload}", device.DeviceId, payload);
                    return;
                }

                // Save to database using a scoped context
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.DeviceLocations.Add(new DeviceLocation
                {
                    DeviceId = device.DeviceId,
                    Latitude = lat.Value,
                    Longitude = lon.Value,
                    Timestamp = DateTime.UtcNow
                });

                await db.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Saved location for device {DeviceId}: {Lat}, {Lon}", device.DeviceId, lat.Value, lon.Value);
            };

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithProtocolVersion(MqttProtocolVersion.V500);

            if (useWebSocket)
            {
                var wsScheme = brokerPort == 8884 ? "wss" : "ws";
                optionsBuilder.WithWebSocketServer(o => o.WithUri($"{wsScheme}://{brokerHost}:{brokerPort}/mqtt"));
            }
            else
            {
                optionsBuilder.WithTcpServer(brokerHost, brokerPort);
            }

            var options = optionsBuilder.Build();

            // Retry loop for initial connection
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _mqttClient.ConnectAsync(options, stoppingToken);
                    _logger.LogInformation("Connected to MQTT broker at {Host}:{Port} (WebSocket: {Ws})", brokerHost, brokerPort, useWebSocket);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to MQTT broker. Retrying in 10 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }

            // Load devices and subscribe
            await RefreshSubscriptionsAsync(stoppingToken);

            // Keep the service alive and periodically refresh device list every 60 seconds
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                    await RefreshSubscriptionsAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during subscription refresh.");
                }
            }
        }

        private async Task RefreshSubscriptionsAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected) return;

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var devices = await db.TrackingDevices.AsNoTracking().ToListAsync(cancellationToken);

            // Subscribe to any new topics
            foreach (var device in devices)
            {
                if (!_trackedDevices.Any(d => d.DeviceId == device.DeviceId && d.MqttTopic == device.MqttTopic))
                {
                    var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                        .WithTopicFilter(f => f.WithTopic(device.MqttTopic))
                        .Build();

                    await _mqttClient.SubscribeAsync(subscribeOptions, cancellationToken);
                    _logger.LogInformation("Subscribed to MQTT topic '{Topic}' for device {DeviceId}", device.MqttTopic, device.DeviceId);
                }
            }

            _trackedDevices = devices;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions
                {
                    Reason = MqttClientDisconnectOptionsReason.NormalDisconnection
                }, cancellationToken);
                _logger.LogInformation("Disconnected from MQTT broker.");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
