namespace Airline1.Dtos.Responses
{
    public class FlightBundleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        public decimal PriceIncrement { get; set; }

        public int CarryOnWeightKg { get; set; }
        public int CheckedBaggagePcs { get; set; }
        public int CheckedBaggageWeightKg { get; set; }

        public bool IncludesPreferredSeatSelection { get; set; }

        public int ChangeFeeType { get; set; }
        public bool IsCancellable { get; set; }
        public bool AllowsTravelFundConversion { get; set; }

        public string? Tagline { get; set; }
    }
}
