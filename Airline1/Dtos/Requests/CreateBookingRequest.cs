namespace Airline1.Dtos.Requests
{
    public class PassengerForBookingDto
    {
        // either PassengerId (existing) OR guest details below
        public int? PassengerId { get; set; }

        // guest fields (optional if PassengerId provided)
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public required string SeatNumber { get; set; } // requested seat
        public bool IsContinuingPassenger { get; set; } = false;
    }

    public class CreateBookingRequest
    {
        public int? UserId { get; set; }       // if logged-in, else null
        public required int FlightId { get; set; }
        public decimal? TotalAmount { get; set; } = 0m;
        public List<PassengerForBookingDto> Passengers { get; set; } = [];
    }
}
