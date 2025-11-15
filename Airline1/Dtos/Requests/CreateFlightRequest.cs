namespace Airline1.Dtos.Requests
{
    public class CreateFlightRequest
    {
        public required string FlightNumber { get; set; }
        public required int AircraftId { get; set; }
        public required int RouteId { get; set; }
        public required DateTime DepartureTime { get; set; }
        public required DateTime ArrivalTime { get; set; }
        public required decimal Price { get; set; }
    }
}
