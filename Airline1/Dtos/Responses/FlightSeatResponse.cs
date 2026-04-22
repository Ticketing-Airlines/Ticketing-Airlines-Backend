namespace Airline1.Dtos.Responses
{
    public class FlightSeatResponse
    {
        public Guid Id { get; set; }
        public int FlightId { get; set; }
        public Guid SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty; 
        public string SeatClass { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? BookingId { get; set; }
        public Guid? PassengerId { get; set; }
        public int? SeatAddOnId { get; set; }
        public decimal? PriceAmount { get; set; }
        public string? PriceCurrency { get; set; }
    }
}
