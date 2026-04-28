namespace Airline1.Dtos.Responses
{
    public class WeatherInfoResponse
    {
        public required WeatherLocationResponse Departure { get; set; }
        public required WeatherLocationResponse Arrival { get; set; }
    }

    public class WeatherLocationResponse
    {
        public required string Temp { get; set; }
        public required string Condition { get; set; }
    }
}
