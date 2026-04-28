namespace Airline1.Dtos.Responses
{
    public class AirportInfoResponse
    {
        public required string Code { get; set; }
        public required string Airport { get; set; }
        public required string City { get; set; }
        public string? Terminal { get; set; }
        public string? Gate { get; set; }
        public required string ScheduledTime { get; set; }
        public string? ActualTime { get; set; }
        public string? EstimatedTime { get; set; }
    }
}
