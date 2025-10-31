namespace Airline1.Dtos.Requests
{
    public class FlightPriceRequest
    {
        public int FlightId { get; set; }
        public decimal BasePrice { get; set; }
        public string CabinClass { get; set; } = "Economy";
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
