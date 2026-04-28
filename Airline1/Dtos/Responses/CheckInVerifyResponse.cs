using System.Collections.Generic;

namespace Airline1.Dtos.Responses
{
    public class CheckInVerifyResponse
    {
        public required string BookingReference { get; set; }
        public bool IsVerified { get; set; }
        public string? ErrorCode { get; set; }
        public BookingDetailsForCheckIn? Booking { get; set; }
        public List<PassengerForCheckIn>? Passengers { get; set; }
        public CheckInEligibilityInfo? Eligibility { get; set; }
    }

    public class BookingDetailsForCheckIn
    {
        public required string Pnr { get; set; }
        public required string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public required string Currency { get; set; }
        public List<FlightInBooking>? Flights { get; set; }
    }

    public class FlightInBooking
    {
        public required string FlightNumber { get; set; }
        public required string Origin { get; set; }
        public required string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }

    public class PassengerForCheckIn
    {
        public Guid PassengerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public required string PassengerType { get; set; }
        public string? SeatNumber { get; set; }
        public bool IsCheckedIn { get; set; }
    }

    public class CheckInEligibilityInfo
    {
        public bool IsEligible { get; set; }
        public DateTime CheckInOpensAt { get; set; }
        public DateTime CheckInClosesAt { get; set; }
        public string? Reason { get; set; }
    }
}