using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    // Represents an individual passenger and their choices within a Booking
    public class BookingPassenger
    {
        [Key]
        public int BookingPassengerId { get; set; }

        public required int BookingId { get; set; }

        // --- Identity ---
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public required string Gender { get; set; } // M, F, O

        // --- Passenger Type (ADT, CHD, INF, SENIOR) ---
        [MaxLength(10)]
        public required string PassengerType { get; set; } = "ADT";

        // --- Seat Assignment ---
        // Nullable foreign key to FlightSeat
        public int? FlightSeatId { get; set; }

        // --- Add-Ons ---
        // List of AddOnPrice IDs purchased for THIS passenger (e.g., extra bags, meals)
        public ICollection<BookingAddOn> AddOns { get; set; } = [];

        // --- Navigation Properties ---
        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [ForeignKey(nameof(FlightSeatId))]
        public FlightSeat? FlightSeat { get; set; }

        // Optional: Link back to a user profile if applicable (e.g., frequent flyer)
        public int? UserId { get; set; }
    }
}