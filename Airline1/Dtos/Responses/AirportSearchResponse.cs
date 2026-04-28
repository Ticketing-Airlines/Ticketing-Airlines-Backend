namespace Airline1.Dtos.Responses
{
    public class AirportSearchResponse
    {
        public int AirportId { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public string? CountryIso2 { get; set; }
        public required string IataCode { get; set; }
    }
}
