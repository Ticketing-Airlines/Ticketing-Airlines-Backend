namespace Airline1.Dtos.Responses
{
    public class AirlineSearchResponse
    {
        public int AirlineId { get; set; }
        public required string Name { get; set; }
        public required string IataCode { get; set; }
        public string? Logo { get; set; }
    }
}
