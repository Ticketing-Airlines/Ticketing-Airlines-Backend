using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class TermsCondition
    {
        [Key]
        public int Id { get; set; }

        public int SeatSaleConfigId { get; set; }

        [Required, MaxLength(50)]
        public string Icon { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        [ForeignKey(nameof(SeatSaleConfigId))]
        public SeatSaleConfig? SeatSaleConfig { get; set; }
    }
}
