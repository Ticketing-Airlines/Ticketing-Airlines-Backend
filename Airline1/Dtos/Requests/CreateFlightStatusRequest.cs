using System.ComponentModel.DataAnnotations;
using Airline1.Common;

namespace Airline1.Dtos.Requests
{
    public class CreateFlightStatusRequest
    {
        public required int FlightId { get; set; }

        public required FlightStatusType Status { get; set; }

        public int? ReasonId { get; set; }

        // when this status is effective (null => now)
        public DateTime? EffectiveAt { get; set; }

        // who performed the change (ops username, system, etc.)
        public string? UpdatedBy { get; set; }
    }
}
