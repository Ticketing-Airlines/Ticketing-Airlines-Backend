namespace Airline1.Dtos.Requests
{
    public class UpdatePassengerForBookingDto
    {
        public int? PassengerId { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public required string SeatNumber { get; set; }
        public bool IsContinuingPassenger { get; set; } = false;
    }

    public class UpdateBookingRequest
    {
        // Allow updating total amount, status (optional), and passenger list (optional)
        public decimal? TotalAmount { get; set; }
        public Airline1.Models.BookingStatus? Status { get; set; }

        // If present, replace the booking's passenger list with this list
        public List<PassengerForBookingDto>? Passengers { get; set; }
    }
}
