using Airline1.Common;

namespace Airline1.Dtos.Responses
{
    public class FlightStatusResponse
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public FlightStatusType Status { get; set; }
        public int? ReasonId { get; set; }
        public string? ReasonCode { get; set; }
        public string? ReasonTitle { get; set; }
        public DateTime EffectiveAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
