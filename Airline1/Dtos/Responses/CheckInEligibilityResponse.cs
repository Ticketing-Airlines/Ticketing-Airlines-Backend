namespace Airline1.Dtos.Responses
{
    public class CheckInEligibilityResponse
    {
        public required string BookingReference { get; set; }
        public bool IsEligible { get; set; }
        public string? Message { get; set; }
        public FlightInfoForCheckIn? Flight { get; set; }
        public CheckInWindow? CheckInWindow { get; set; }
    }

    public class FlightInfoForCheckIn
    {
        public required string FlightNumber { get; set; }
        public required string Origin { get; set; }
        public required string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public required string FlightType { get; set; }
    }

    public class CheckInWindow
    {
        public DateTime OpensAt { get; set; }
        public DateTime ClosesAt { get; set; }
    }
}