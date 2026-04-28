using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    // Represents a boarding pass issued after check-in
    public class BoardingPass
    {
        [Key]
        public Guid BoardingPassId { get; set; } = Guid.NewGuid();

        public required Guid CheckInId { get; set; }

        [MaxLength(6)]
        public required string BookingReference { get; set; }

        [MaxLength(20)]
        public required string FlightNumber { get; set; }

        [MaxLength(100)]
        public required string PassengerName { get; set; }

        [MaxLength(10)]
        public required string FromAirport { get; set; }

        [MaxLength(10)]
        public required string ToAirport { get; set; }

        public DateTime DepartureDate { get; set; }

        [MaxLength(10)]
        public required string SeatNumber { get; set; }

        [MaxLength(100)]
        public required string Barcode { get; set; }

        public string? QRCodeImageBase64 { get; set; }

        [MaxLength(500)]
        public string? BoardingPassUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CheckInId))]
        public CheckIn? CheckIn { get; set; }
    }
}