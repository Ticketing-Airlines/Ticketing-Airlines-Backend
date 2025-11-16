using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Airline_Ticketing.Enums;

namespace Airline_Ticketing.Model
{
    

    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        // Foreign keys
        public int UserID { get; set; }
        public int FlightID { get; set; }

        // Booking reference code for users
        [Required]
        [MaxLength(20)]
        public string BookingCode { get; set; } = string.Empty; // e.g., "BK-20251027-ABC123"

        public DateOnly BookingDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
    

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Timestamps for auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties (for eager loading)
        public Users? User { get; set; }
        public Flights? Flight { get; set; }

        // Multiple passengers per booking
        public ICollection<BookingPassengers> Passengers { get; set; } = new List<BookingPassengers>();
    }
}