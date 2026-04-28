namespace Airline1.Dtos.Responses
{
    public class FlightStatusLookupResponse
    {
        public required string FlightNumber { get; set; }
        public required string Airline { get; set; }
        public required string Aircraft { get; set; }
        public required string Date { get; set; }
        public required string Status { get; set; }
        public required string Duration { get; set; }
        public required AirportInfoResponse Departure { get; set; }
        public required AirportInfoResponse Arrival { get; set; }
        public required WeatherInfoResponse Weather { get; set; }
    }
}
