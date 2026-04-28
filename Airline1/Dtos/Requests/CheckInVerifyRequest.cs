namespace Airline1.Dtos.Requests
{
    public class CheckInVerifyRequest
    {
        public required string BookingReference { get; set; }
        public required string LastName { get; set; }
    }
}