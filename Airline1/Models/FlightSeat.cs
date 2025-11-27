using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class FlightSeat
    {
        [Key]
        public int FlightSeatId { get; set; }

        public required int FlightId { get; set; }

        public required int SeatId { get; set; } // references Seat table (physical seat)

        // --- Inventory Grouping ---
        [MaxLength(50)]
        public required string SeatClass { get; set; } = "Economy";

        // --- Status (Booking state) ---
        [MaxLength(50)]
        public required string Status { get; set; } = "Available"; // Available, Booked, Blocked, CheckedIn

        // --- Transactional Links ---
        public int? BookingId { get; set; }
        public int? PassengerId { get; set; } // Note: This field is updated by the service logic

        // --- Pricing Link (e.g., seat add-on id) ---
        public int? SeatAddOnId { get; set; }

        public BookingPassenger? BookingPassenger { get; set; } // This completes the relationship: .WithOne(fs => fs.BookingPassenger)

        [ForeignKey(nameof(FlightId))]
        public Flight? Flight { get; set; }

        [ForeignKey(nameof(SeatId))]
        public Seat? Seat { get; set; }

        [ForeignKey(nameof(SeatAddOnId))]
        public FlightAddOn? SeatAddOn { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}