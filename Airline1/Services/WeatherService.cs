using System.Text.Json;
using Airline1.IService;

namespace Airline1.Services
{
    public class WeatherService(IConfiguration configuration, ILogger<WeatherService> logger) : IWeatherService
    {
        private readonly HttpClient _httpClient = new();
        private readonly string? _apiKey = configuration["WeatherApi:Key"];
        private readonly string _baseUrl = configuration["WeatherApi:BaseUrl"] ?? "https://api.openweathermap.org/data/2.5";

        public async Task<WeatherResult> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                logger.LogWarning("Weather API key not configured. Using fallback weather data.");
                return GetFallbackWeather();
            }

            try
            {
                var url = $"{_baseUrl}/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Weather API returned status {StatusCode}. Using fallback.", response.StatusCode);
                    return GetFallbackWeather();
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var temp = root.GetProperty("main").GetProperty("temp").GetDouble();
                var condition = root.GetProperty("weather")[0].GetProperty("main").GetString() ?? "Unknown";
                var description = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? condition;

                // Capitalize first letter of each word
                var formattedCondition = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(description);

                return new WeatherResult
                {
                    Temp = $"{Math.Round(temp)}°C",
                    Condition = formattedCondition
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch weather data. Using fallback.");
                return GetFallbackWeather();
            }
        }

        private static WeatherResult GetFallbackWeather()
        {
            var conditions = new[] { "Sunny", "Partly Cloudy", "Cloudy", "Light Rain" };
            var random = new Random();
            var temp = random.Next(24, 35);

            return new WeatherResult
            {
                Temp = $"{temp}°C",
                Condition = conditions[random.Next(conditions.Length)]
            };
        }
    }
}
