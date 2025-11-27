using Airline1.Models;

namespace Airline1.Dtos.Responses
{
    public class FlightAddOnResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public AddOnCategory Category { get; set; }
        public string? Description { get; set; }
        public int? WeightKg { get; set; }
        public int? PieceCount { get; set; }
        public bool IsPremiumSeatType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
