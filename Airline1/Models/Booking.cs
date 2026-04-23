using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    // Represents the entire flight reservation transaction
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; } = Guid.NewGuid();

        // PNR-like record locator
        [MaxLength(6)]
        public required string Pnr { get; set; }

        // --- Flight and Bundle Details ---
        public ICollection<BookingFlight> BookingFlights { get; set; } = new List<BookingFlight>(); 
        public required int FlightBundleId { get; set; }

        // --- User/Guest Information ---
        public int? UserId { get; set; } // Null for Guest booking

        [MaxLength(255)]
        public required string ContactEmail { get; set; }

        [MaxLength(20)]
        public required string ContactPhone { get; set; }

        // --- Financials ---
        [Column(TypeName = "decimal(18, 2)")]
        public required decimal TotalPrice { get; set; }

        [MaxLength(5)]
        public required string Currency { get; set; } = "PHP";

        // --- Status and Audit ---
        [MaxLength(50)]
        public required string Status { get; set; } = "PendingPayment"; // PendingPayment, Confirmed, Cancelled, Completed

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // GCash, PayMaya, CreditCard

        [MaxLength(100)]
        public string? PaymentReference { get; set; } // Transaction reference from payment gateway

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public DateTime? PaymentDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public FlightBundle? FlightBundle { get; set; }

        // One-to-Many relationship with passengers
        public ICollection<BookingPassenger> Passengers { get; set; } = [];
    }
}