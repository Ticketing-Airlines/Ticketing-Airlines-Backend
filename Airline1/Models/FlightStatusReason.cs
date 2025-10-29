using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class FlightStatusReason
    {
        [Key]
        public int Id { get; set; }

        // short machine-friendly code, e.g. "WEATHER", "TECHNICAL"
        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        // human-friendly description, reused by flight notifications
        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // optional category / tag (for filtering in UI)
        [MaxLength(100)]
        public string? Category { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
