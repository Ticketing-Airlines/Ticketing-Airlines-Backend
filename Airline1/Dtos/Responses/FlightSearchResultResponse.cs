namespace Airline1.Dtos.Responses
{
    public class FlightSearchResultResponse
    {
        public int FlightInstanceId { get; set; }
        public required string FlightNumber { get; set; }
        public AirportSearchResponse? OriginAirport { get; set; }
        public AirportSearchResponse? DestinationAirport { get; set; }
        public required string DepartureTime { get; set; }
        public required string ArrivalTime { get; set; }
        public required string Duration { get; set; }
        public decimal Price { get; set; }
        public required string Currency { get; set; }
        public string? FareCode { get; set; }
        public int AvailableSeats { get; set; }
        public AircraftSearchResponse? Aircraft { get; set; }
        public AirlineSearchResponse? Airline { get; set; }
    }
}
