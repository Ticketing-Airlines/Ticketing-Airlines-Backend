using System;
using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class FlightStatusReason
    {
        [Key]
        public int Id { get; set; }

        // short machine-friendly code, unique
        [MaxLength(50)]
        public required string Code { get; set; } = string.Empty;

        // human friendly title
        [ MaxLength(200)]
        public required string Title { get; set; } = string.Empty;

        // longer description or template used in notifications
        [MaxLength(1000)]
        public string? Description { get; set; }

        // whether this reason is currently available for selection
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
