using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    // Request to initiate a new booking
    public class CreateBookingRequest
    {
        [Required]
        public int FlightId { get; set; }

        [Required]
        public int FlightBundleId { get; set; }

        // --- Contact Info ---
        [Required, EmailAddress]
        public string ContactEmail { get; set; } = null!;

        [Required, Phone]
        public string ContactPhone { get; set; } = null!;

        // --- User/Guest Info ---
        public int? UserId { get; set; } // User ID if a registered user is making the booking

        // List of passengers for this booking
        [Required]
        public List<CreateBookingPassengerRequest> Passengers { get; set; } = [];
    }

    // Individual passenger details for creation
    public class CreateBookingPassengerRequest
    {
        [MaxLength(50)]
        public required string FirstName { get; set; } = null!;

        [MaxLength(50)]
        public required  string LastName { get; set; } = null!;

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        public required System.DateTime DateOfBirth { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; } = "M";

        // ADT, CHD, SENIOR, INFANT
        [MaxLength(10)]
        public required string PassengerType { get; set; } = "ADT";

        // Optional: Selected seat for this passenger
        public int? FlightSeatId { get; set; }

        // Optional: List of AddOnPrice IDs (which contain FlightAddOn and FlightId context)
        public List<int> AddOnPriceIds { get; set; } = [];
    }
}