using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    // Defines the price of a specific Add-On product for a specific flight
    public class AddOnPrice
    {
        [Key]
        public int AddOnPriceId { get; set; }

        // The specific flight this price rule applies to
        public required int FlightId { get; set; }

        // The product being priced (links to the FlightAddOn master catalog)
        public required int AddOnId { get; set; }

        // --- Pricing Details ---

        // The actual amount charged for the add-on 
        [Column(TypeName = "decimal(18, 2)")] // Ensures correct precision for currency
        public required decimal PriceAmount { get; set; }

        // The currency of the price (e.g., "PHP", "USD")
        [MaxLength(5)]
        public required string Currency { get; set; } = "PHP";

        // --- Validity Period (Temporal Integrity) ---

        // When this price rule becomes effective
        public required DateTime ValidFrom { get; set; } = DateTime.UtcNow;

        // When this price rule is no longer valid (NULL means it's permanent/indefinite)
        public DateTime? ValidTo { get; set; }

        // --- Audit Fields ---
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // --- Navigation Properties ---

        [ForeignKey(nameof(FlightId))]
        public Flight? Flight { get; set; }

        [ForeignKey(nameof(AddOnId))]
        public FlightAddOn? AddOn { get; set; }
    }
}