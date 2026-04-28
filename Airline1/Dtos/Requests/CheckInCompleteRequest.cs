namespace Airline1.Dtos.Requests
{
    public class CheckInCompleteRequest
    {
        public required string BookingReference { get; set; }
        public required List<PassengerSeatAssignment> Passengers { get; set; }
    }
}