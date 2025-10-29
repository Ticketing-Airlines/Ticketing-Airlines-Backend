using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string BookingCode { get; set; } = string.Empty; // e.g. PR-20251024-ABC123

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }               // optional (guest booking)
        public User? User { get; set; }

        [Required]
        public int FlightId { get; set; }              // flight being booked
        public Flight Flight { get; set; } = null!;

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public decimal TotalAmount { get; set; } = 0m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<BookingPassenger> Passengers { get; set; } = [];
    }
}
