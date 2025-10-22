namespace Airline1.Dtos.Responses
{
    public class FlightResponse
    {
        public int Id { get; set; }
        public required string FlightNumber { get; set; }
        public string? AircraftName { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
    }
}
