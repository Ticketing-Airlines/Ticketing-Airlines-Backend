using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class SeatSale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Destination { get; set; } = string.Empty;

        [Required, MaxLength(3)]
        public string DestinationAirportCode { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required, MaxLength(2)]
        public string CountryIso2 { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Image { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public int Discount { get; set; }

        [MaxLength(5)]
        public string Currency { get; set; } = "PHP";

        [MaxLength(100)]
        public string PriceNote { get; set; } = string.Empty;

        public DateOnly TravelPeriodStart { get; set; }

        public DateOnly TravelPeriodEnd { get; set; }

        public DateOnly BookingDeadline { get; set; }

        public int SeatsLeft { get; set; }

        public int TotalSeats { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Features { get; set; } = "[]";

        public bool Featured { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
