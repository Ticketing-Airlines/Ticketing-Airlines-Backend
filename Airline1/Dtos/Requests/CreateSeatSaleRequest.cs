using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class CreateSeatSaleRequest
    {
        [Required, MaxLength(100)]
        public string Destination { get; set; } = null!;

        [Required, MaxLength(3)]
        public string DestinationAirportCode { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Country { get; set; } = null!;

        [Required, MaxLength(2)]
        public string CountryIso2 { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Type { get; set; } = null!;

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? Image { get; set; }

        [Required]
        public decimal OriginalPrice { get; set; }

        [Required]
        public decimal SalePrice { get; set; }

        [Required]
        public int Discount { get; set; }

        [MaxLength(5)]
        public string Currency { get; set; } = "PHP";

        [MaxLength(100)]
        public string? PriceNote { get; set; }

        [Required]
        public DateOnly TravelPeriodStart { get; set; }

        [Required]
        public DateOnly TravelPeriodEnd { get; set; }

        [Required]
        public DateOnly BookingDeadline { get; set; }

        [Required]
        public int SeatsLeft { get; set; }

        [Required]
        public int TotalSeats { get; set; }

        public string Features { get; set; } = "[]";

        public bool Featured { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}
