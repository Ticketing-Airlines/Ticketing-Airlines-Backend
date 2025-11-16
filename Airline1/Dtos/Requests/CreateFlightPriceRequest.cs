namespace Airline1.Dtos.Requests
{
    public class CreateFlightPriceRequest
    {
        public int FlightId { get; set; }
        public string CabinClass { get; set; } = "Economy";
        public decimal BasePrice { get; set; }

        // Type: "Standard" or "Promo"
        public string Type { get; set; } = "Standard";

        // schedule window for promo or scheduled standard
        public DateTime? EffectiveFrom { get; set; }  // default UtcNow if null
        public DateTime? EffectiveTo { get; set; }    // null = open ended

        public string? UpdatedBy { get; set; }
        public string? Note { get; set; }
    }
}
