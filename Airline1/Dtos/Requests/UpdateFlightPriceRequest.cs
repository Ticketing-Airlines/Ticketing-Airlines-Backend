using System;
using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class UpdateFlightPriceRequest
    {
        [Range(0.01, 99999.99)]
        public decimal? BasePrice { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Note { get; set; }
    }
}