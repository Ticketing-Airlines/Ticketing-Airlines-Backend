using System;
using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class CreateFlightPriceRequest
    {
        public required int FlightId { get; set; }

        public required int FlightBundleId { get; set; } 

        [MaxLength(50)]
        public string CabinClass { get; set; } = "Economy";

        [Range(0.01, 99999.99)]
        public required decimal BasePrice { get; set; }

        [MaxLength(10)]
        public required string PassengerType { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public string? UpdatedBy { get; set; }
        public string? Note { get; set; }
    }
}