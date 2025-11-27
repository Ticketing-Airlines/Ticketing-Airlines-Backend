using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public enum AddOnCategory { Baggage = 1, Seat = 2, Meal = 3, Insurance = 4, Other = 99 }

    public class FlightAddOn
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; } = null!; // e.g., "Extra 4KG Checked Bag", "Exit Row Seat"

        [MaxLength(20)]
        public required string Code { get; set; } = null!; // e.g., "BAG_4KG", "SEAT_EXIT"

        [Required]
        public AddOnCategory Category { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        // Baggage/Weight Detail (if Category is Baggage)
        public int? WeightKg { get; set; }
        public int? PieceCount { get; set; }

        // Seat Detail (if Category is Seat)
        public bool IsPremiumSeatType { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}