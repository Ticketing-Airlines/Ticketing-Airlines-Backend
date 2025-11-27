using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class BookingPassenger
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;

        // Denormalized FlightId for easy queries (keeps checks simple)
        public int FlightId { get; set; }

        // Link to existing passenger (if provided), otherwise the service will create a Passenger record
        [ForeignKey(nameof(Passenger))]
        public int? PassengerId { get; set; }
        public Passenger? Passenger { get; set; }

        [MaxLength(150)]
        public string PassengerName { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? PassengerEmail { get; set; }

        [MaxLength(6)]
        public string SeatNumber { get; set; } = string.Empty; 

        public bool IsContinuingPassenger { get; set; } = false; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
