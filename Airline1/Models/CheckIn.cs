using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    // Represents a passenger check-in for a booking
    public class CheckIn
    {
        [Key]
        public Guid CheckInId { get; set; } = Guid.NewGuid();

        public required Guid BookingId { get; set; }

        // Nullable because check-in is per booking but can track individual passengers
        public Guid? PassengerId { get; set; }

        [MaxLength(10)]
        public string? SeatNumber { get; set; }

        [MaxLength(100)]
        public string? Barcode { get; set; }

        [MaxLength(500)]
        public string? QRCode { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [ForeignKey(nameof(PassengerId))]
        public BookingPassenger? Passenger { get; set; }

        public BoardingPass? BoardingPass { get; set; }
    }
}