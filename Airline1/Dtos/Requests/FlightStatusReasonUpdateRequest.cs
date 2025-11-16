namespace Airline1.Dtos.Requests
{
    public class FlightStatusReasonUpdateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
