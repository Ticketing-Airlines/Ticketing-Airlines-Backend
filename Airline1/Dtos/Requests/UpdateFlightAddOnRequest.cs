using System.ComponentModel.DataAnnotations;
using Airline1.Models;

namespace Airline1.Dtos.Requests
{
    public class UpdateFlightAddOnRequest
    {
        [MaxLength(50)]
        public required string Name { get; set; } = null!;

        [MaxLength(20)]
        public required string Code { get; set; } = null!;

        [Required]
        public AddOnCategory Category { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public int? WeightKg { get; set; }
        public int? PieceCount { get; set; }
        public bool IsPremiumSeatType { get; set; }
    }
}
