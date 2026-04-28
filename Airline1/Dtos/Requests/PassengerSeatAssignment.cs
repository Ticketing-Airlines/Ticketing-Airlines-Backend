namespace Airline1.Dtos.Requests
{
    public class PassengerSeatAssignment
    {
        public required Guid PassengerId { get; set; }
        public required string SeatNumber { get; set; }
    }
}