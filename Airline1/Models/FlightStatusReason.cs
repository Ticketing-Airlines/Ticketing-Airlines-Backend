using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class FlightStatusReason
    {
        [Key]
        public int Id { get; set; }

        // short machine-friendly code, e.g. "WEATHER", "TECHNICAL"
        [MaxLength(50)]
        public required string Code { get; set; } = string.Empty;

        [MaxLength(500)]
        public required string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Category { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
