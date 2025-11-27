// Models/FlightPrice.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class FlightPrice
    {
        [Key]
        public int Id { get; set; }
        public required int FlightId { get; set; }

        public required int FlightBundleId { get; set; }

        [ForeignKey(nameof(FlightId))]
        public Flight? Flight { get; set; }

        [ForeignKey(nameof(FlightBundleId))]
        public FlightBundle? FlightBundle { get; set; }

        [MaxLength(50)]
        public required string CabinClass { get; set; } = "Economy";

        // 🌟 NEW: Passenger Type Code (e.g., ADT, CHD, INF)
        [MaxLength(10)]
        public required string PassengerType { get; set; } = "ADT";

        [Column(TypeName = "decimal(10,2)")]
        public required decimal BasePrice { get; set; }

        // Temporal control
        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
        public string? Note { get; set; }

        [NotMapped]
        public bool IsActive => (EffectiveTo == null || EffectiveTo > DateTime.UtcNow) && EffectiveFrom <= DateTime.UtcNow;
    }
}