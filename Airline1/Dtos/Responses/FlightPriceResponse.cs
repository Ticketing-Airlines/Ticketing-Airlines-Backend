using System;

namespace Airline1.Dtos.Responses
{
    public class FlightPriceResponse
    {
        public int Id { get; set; }
        public int FlightId { get; set; }

        // ⭐ NEW: Added this back for context (to match mapping)
        public string? FlightNumber { get; set; }

        // Replacement for the obsolete 'Type'
        public int FlightBundleId { get; set; }
        public string? BundleName { get; set; } // Mapped from FlightBundle nav property

        public string CabinClass { get; set; } = "Economy";
        public decimal BasePrice { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }

        public string? Note { get; set; }
    }
}