using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Airline1.Common;

namespace Airline1.Models
{
    public class FlightStatus
    {
        [Key]
        public int Id { get; set; }

        public required int FlightId { get; set; }

        [ForeignKey(nameof(FlightId))]
        public Flight? Flight { get; set; }

        public required FlightStatusType Status { get; set; } = FlightStatusType.Scheduled;

        // only ReasonId allowed (predefined reasons), no free-text
        public int? ReasonId { get; set; }

        [ForeignKey(nameof(ReasonId))]
        public FlightStatusReason? Reason { get; set; }

        // when this status becomes effective
        public DateTime EffectiveAt { get; set; } = DateTime.UtcNow;
        public bool IsCurrent { get; set; }
        // audit
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
