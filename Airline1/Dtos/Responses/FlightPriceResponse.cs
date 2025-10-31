namespace Airline1.Dtos.Responses
{
    public class FlightPriceResponse
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string CabinClass { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Type { get; set; } = "Standard";
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Note { get; set; }
    }
}
