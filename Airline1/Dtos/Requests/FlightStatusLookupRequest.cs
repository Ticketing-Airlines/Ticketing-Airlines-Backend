namespace Airline1.Dtos.Requests
{
    public class FlightStatusLookupRequest
    {
        public required string FlightNumber { get; set; }
        public required string Date { get; set; }
    }
}
