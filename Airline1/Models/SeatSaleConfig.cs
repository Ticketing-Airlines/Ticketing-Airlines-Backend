using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class SeatSaleConfig
    {
        [Key]
        public int Id { get; set; }

        public DateTime SaleEndDate { get; set; }

        [Required, MaxLength(100)]
        public string SaleTitle { get; set; } = string.Empty;

        [MaxLength(255)]
        public string SaleSubtitle { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [MaxLength(100)]
        public string HeroMessage { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Version { get; set; } = "1.0";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<TermsCondition> TermsAndConditions { get; set; } = new List<TermsCondition>();
    }
}
