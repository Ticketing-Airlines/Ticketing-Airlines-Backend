using Airline_Ticketing.Enums;

namespace Airline_Ticketing.DTOs.Response
{
    public class BookingResponse
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int FlightID { get; set; }
        public string BookingCode { get; set; }
        public string? FlightNumber { get; set; }
        public DateOnly BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
