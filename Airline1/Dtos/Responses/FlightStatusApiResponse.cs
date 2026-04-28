namespace Airline1.Dtos.Responses
{
    public class FlightStatusApiResponse
    {
        public bool Success { get; set; }
        public FlightStatusLookupResponse? Data { get; set; }
        public string? Error { get; set; }
        public string? Message { get; set; }
    }
}
