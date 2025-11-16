namespace Airline1.Dtos.Requests
{
    public class FlightStatusReasonCreateRequest
    {
        public required string Code { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; } // optional; defaults to true
    }
}
