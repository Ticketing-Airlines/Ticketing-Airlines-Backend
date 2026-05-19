using Xunit;
using Moq;
using Airline1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;

namespace Airline1.Tests.Services
{
    public class WeatherServiceTests
    {
        private readonly Mock<ILogger<WeatherService>> _mockLogger;

        public WeatherServiceTests()
        {
            _mockLogger = new Mock<ILogger<WeatherService>>();
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_ReturnsFallback_WhenNoApiKeyConfigured()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();
            var service = new WeatherService(config, _mockLogger.Object);

            // Act
            var result = await service.GetCurrentWeatherAsync(14.5995, 120.9842);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Temp);
            Assert.NotNull(result.Condition);
            Assert.Contains("°C", result.Temp);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_ReturnsFallback_WhenApiRequestFails()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "WeatherApi:Key", "test_api_key" },
                    { "WeatherApi:BaseUrl", "http://nonexistent.invalid.domain" }
                })
                .Build();
            var service = new WeatherService(config, _mockLogger.Object);

            // Act
            var result = await service.GetCurrentWeatherAsync(14.5995, 120.9842);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Temp);
            Assert.NotNull(result.Condition);
            Assert.Contains("°C", result.Temp);
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_ReturnsValidResult_WhenApiSucceeds()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "WeatherApi:Key", "valid_test_key" },
                    { "WeatherApi:BaseUrl", "https://api.openweathermap.org/data/2.5" }
                })
                .Build();
            var service = new WeatherService(config, _mockLogger.Object);

            // Act
            var result = await service.GetCurrentWeatherAsync(14.5995, 120.9842);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Temp);
            Assert.NotNull(result.Condition);
            Assert.Contains("°C", result.Temp);
        }
    }
}