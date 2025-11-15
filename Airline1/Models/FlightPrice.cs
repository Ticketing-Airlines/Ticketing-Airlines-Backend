using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public enum FlightPriceType { Standard = 0, Promo = 1 }

    public class FlightPrice
    {
        [Key]
        public int Id { get; set; }

        public required int FlightId { get; set; }

        [ForeignKey(nameof(FlightId))]
        public Flight? Flight { get; set; }

        [Required, MaxLength(50)]
        public string CabinClass { get; set; } = "Economy";

        [Column(TypeName = "decimal(10,2)")]
        public required decimal BasePrice { get; set; }

        // Standard or Promo
        public FlightPriceType Type { get; set; } = FlightPriceType.Standard;

        // promo/price window (promo uses these)
        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public string? Note { get; set; } // e.g. "Autumn promo 20% off"

        // convenience
        [NotMapped]
        public bool IsActive => (EffectiveTo == null || EffectiveTo > DateTime.UtcNow) && EffectiveFrom <= DateTime.UtcNow;
    }
}
