namespace Airline1.Dtos.Responses
{
    public class AircraftSearchResponse
    {
        public int AircraftId { get; set; }
        public required string Model { get; set; }
        public required string Manufacturer { get; set; }
        public int Capacity { get; set; }
    }
}
