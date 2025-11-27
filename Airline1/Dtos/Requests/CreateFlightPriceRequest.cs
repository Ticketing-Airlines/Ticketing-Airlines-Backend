using System;
using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class CreateFlightPriceRequest
    {
        [Required]
        public required int FlightId { get; set; }

        [Required]
        public required int FlightBundleId { get; set; } 

        [Required, MaxLength(50)]
        public string CabinClass { get; set; } = "Economy";

        [Required, Range(0.01, 99999.99)]
        public required decimal BasePrice { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public string? UpdatedBy { get; set; }
        public string? Note { get; set; }
    }
}