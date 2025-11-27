using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class UpdateAddOnPriceRequest
    {
        public required int FlightId { get; set; }

        public required int AddOnId { get; set; }

        [Range(0.01, 100000.00, ErrorMessage = "Price must be greater than zero.")]
        public required decimal PriceAmount { get; set; }

        [MaxLength(5)]
        public required string Currency { get; set; } = "PHP";

        public required DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}