using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class UpdateFlightBundleRequest
    {
        [ MaxLength(50)]
        public required string Name { get; set; } = null!;

        [MaxLength(20)]
        public required string Code { get; set; } = null!;

        public decimal PriceIncrement { get; set; }

        public int CarryOnWeightKg { get; set; }
        public int CheckedBaggagePcs { get; set; }
        public int CheckedBaggageWeightKg { get; set; }

        public bool IncludesPreferredSeatSelection { get; set; }

        public int ChangeFeeType { get; set; }
        public bool IsCancellable { get; set; }
        public bool AllowsTravelFundConversion { get; set; }

        [MaxLength(255)]
        public string? Tagline { get; set; }
    }
}
