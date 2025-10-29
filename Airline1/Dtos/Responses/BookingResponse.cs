using Airline1.Models;

namespace Airline1.Dtos.Responses
{
    public class BookingPassengerResponse
    {
        public int Id { get; set; }
        public int? PassengerId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string? PassengerEmail { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
    }

    public class BookingResponse
    {
        public int Id { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<BookingPassengerResponse> Passengers { get; set; } = [];
    }
}
