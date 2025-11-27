
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class FlightBundle
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; } = null!; // e.g., "GO Easy"

        [MaxLength(20)]
        public required string Code { get; set; } = null!; // Short code for system use, e.g., "GOE"

        // 💰 Price Increment (The fixed amount added to the FlightPrice.BasePrice, per guest)
        [Column(TypeName = "decimal(10,2)")]
        public required decimal PriceIncrement { get; set; } = 0m;

        // --- Baggage and Seating Entitlements ---

        // Hand-carry bag is included in all, but storing the Max Weight is crucial.
        public int CarryOnWeightKg { get; set; } = 7;

        public int CheckedBaggagePcs { get; set; } = 0;

        public int CheckedBaggageWeightKg { get; set; } = 0;

        //Preferred Seat Selection (Standard seat of your choice)
        public bool IncludesPreferredSeatSelection { get; set; } = false;

        // --- Flexibility and Rules (The high-value differences) ---

        // Change Fees (e.g., 0=Fee applies, 1=Free/Flexible)
        public int ChangeFeeType { get; set; } = 0;

        //  Is the booking eligible for cancellation (e.g., "Enjoy free cancellation")
        public bool IsCancellable { get; set; } = false;

        // Ability to convert to Travel Fund (CEB Flexi)
        public bool AllowsTravelFundConversion { get; set; } = false;

        // Additional display text for marketing/rules summary
        [MaxLength(255)]
        public string? Tagline { get; set; }
    }
}