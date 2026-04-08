using System.Collections.Generic;

namespace Airline1.Dtos.Responses
{
    // Full response for a single booking transaction
    public class BookingResponse
    {
        public int BookingId { get; set; }
        public string Pnr { get; set; } = null!;

        public List<FlightResponse> Flights { get; set; } = new List<FlightResponse>();
        public int FlightBundleId { get; set; }
        public string FlightBundleName { get; set; } = null!; // Display name for the bundle

        public int? UserId { get; set; }
        public string ContactEmail { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;

        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = null!;
        public string Status { get; set; } = null!;

        public System.DateTime BookingDate { get; set; }
        public System.DateTime? PaymentDate { get; set; }

        public List<BookingPassengerResponse> Passengers { get; set; } = [];
    }

    // Response for an individual passenger within a booking
    public class BookingPassengerResponse
    {
        public int PassengerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public System.DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;
        public string PassengerType { get; set; } = null!;

        // Seat Info
        public int? FlightSeatId { get; set; }
        public string? SeatNumber { get; set; }

        // Add-Ons Info
        public List<BookingAddOnResponse> AddOns { get; set; } = [];
    }

    // Response for an add-on purchased in the booking
    public class BookingAddOnResponse
    {
        public int BookingAddOnId { get; set; }
        public int AddOnPriceId { get; set; }
        public string AddOnName { get; set; } = null!;
        public string AddOnCode { get; set; } = null!;
        public decimal PriceAtBooking { get; set; }
    }
}