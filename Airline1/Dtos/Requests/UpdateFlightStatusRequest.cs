using Airline1.Common;

namespace Airline1.Dtos.Requests
{
    public class UpdateFlightStatusRequest
    {
        // optional: if client only wants to change reason or mark final states
        public FlightStatusType? Status { get; set; }
        public int? ReasonId { get; set; }
        public DateTime? EffectiveAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
