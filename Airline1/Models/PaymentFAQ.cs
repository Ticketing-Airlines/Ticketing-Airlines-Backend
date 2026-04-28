using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class PaymentFAQ
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Category { get; set; }

        [MaxLength(255)]
        public required string Question { get; set; }

        public required string Answer { get; set; }

        [MaxLength(50)]
        public required string Icon { get; set; }

        [MaxLength(20)]
        public required string Color { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
