using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class BookingPassengerRequest
    {
        // Fare details
        [Required]
        public required int FlightPriceId { get; set; } // Mandatory: The specific fare/bundle/passenger type price

        // Passenger Identity
        [Required, MaxLength(50)]
        public required string FirstName { get; set; } = null!;
        [MaxLength(50)]
        public string? MiddleName { get; set; }
        [Required, MaxLength(50)]
        public required string LastName { get; set; } = null!;

        [Required]
        public required DateTime DateOfBirth { get; set; }

        // Seat and Add-Ons
        public Guid? FlightSeatId { get; set; } // The specific seat ID (FlightSeat.FlightSeatId)

        public List<int>? AddOnPriceIds { get; set; } = []; // List of purchased AddOnPrice IDs
    }
}