namespace Airline1.Models
{
    public class BookingFlight
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BookingId { get; set; }

        public Booking? Booking { get; set; }

        public int FlightId { get; set; }

        public Flight? Flight { get; set; } 
    }
}
