using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class Airline
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [Required, MaxLength(3)]
        public required string IataCode { get; set; }

        [MaxLength(500)]
        public string? Logo { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
