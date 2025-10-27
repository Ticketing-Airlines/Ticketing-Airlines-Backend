namespace Airline1.Dtos.Responses
{
    public class PassengerResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? PassportExpiry { get; set; }
        public string? SpecialAssistance { get; set; }
        public bool IsActive { get; set; }
        public int? FlightId { get; set; }         // nullable if no flight assigned
        public string? SeatNumber { get; set; }    // e.g. "12A"
        public string? FlightNumber { get; set; }  // convenience (if you want)

    }
}
