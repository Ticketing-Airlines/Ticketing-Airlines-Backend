using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class PaymentMethod
    {
        [Key]
        [MaxLength(50)]
        public required string Id { get; set; }

        [MaxLength(50)]
        public required string Category { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(255)]
        public required string Description { get; set; }

        [MaxLength(50)]
        public required string ProcessingTime { get; set; }

        [MaxLength(20)]
        public required string FeeType { get; set; } // fixed, percentage

        [Column(TypeName = "decimal(10, 2)")]
        public required decimal FeeAmount { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? FeeFixedAmount { get; set; }

        [MaxLength(3)]
        public required string FeeCurrency { get; set; } = "PHP";

        [MaxLength(50)]
        public required string FeeDisplayText { get; set; }

        [MaxLength(20)]
        public required string Color { get; set; }

        public bool Featured { get; set; }

        public bool IsActive { get; set; } = true;

        public string ProvidersJson { get; set; } = "[]";

        [MaxLength(50)]
        public required string Icon { get; set; }

        public string FeaturesJson { get; set; } = "[]";

        public bool IsAvailable { get; set; } = true;

        [MaxLength(255)]
        public string? MaintenanceSchedule { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
