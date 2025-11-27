namespace Airline1.Dtos.Responses
{
    public class FlightSeatResponse
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty; 
        public string SeatClass { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? BookingId { get; set; }
        public int? PassengerId { get; set; }
        public int? SeatAddOnId { get; set; }
    }
}
