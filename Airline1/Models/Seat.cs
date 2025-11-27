using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }

        public required int AircraftId { get; set; }

        [ MaxLength(10)]
        public required string SeatNumber { get; set; } = string.Empty; // "12A"

        [Required, MaxLength(50)]
        public string SeatClass { get; set; } = "Economy"; // Economy, Business, First

        public bool IsExitRow { get; set; } = false;

        public bool IsAvailable { get; set; } = true;

        // Navigation
        [ForeignKey(nameof(AircraftId))]
        public Aircraft? Aircraft { get; set; }
    }
}
