namespace Airline1.Dtos.Requests
{
    public class UpdateFlightRequest
    {
        public string? FlightNumber { get; set; }
        public int? AircraftId { get; set; }
        public int? RouteId { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public decimal? Price { get; set; }
    }
}
