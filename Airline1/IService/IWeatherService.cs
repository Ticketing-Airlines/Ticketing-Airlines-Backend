namespace Airline1.IService
{
    public interface IWeatherService
    {
        Task<WeatherResult> GetCurrentWeatherAsync(double latitude, double longitude);
    }

    public class WeatherResult
    {
        public required string Temp { get; set; }
        public required string Condition { get; set; }
    }
}
