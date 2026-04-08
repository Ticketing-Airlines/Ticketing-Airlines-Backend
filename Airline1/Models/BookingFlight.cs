namespace Airline1.Models
{
    public class BookingFlight
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public Booking? Booking { get; set; }

        public int FlightId { get; set; }

        public Flight? Flight { get; set; } 
    }
}
