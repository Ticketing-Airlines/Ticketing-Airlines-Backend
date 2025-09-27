using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class Airport
    {
        [Key]
        public int Id { get; set; }

        // IATA (3-chars) like MNL
        [Required, MaxLength(3)]
        public required string IataCode { get; set; }

        // ICAO (4-chars) like RPLL
        [MaxLength(4)]
        public required string IcaoCode { get; set; }

        [Required, MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(100)]
        public required string City { get; set; }

        [MaxLength(100)]
        public required string Country { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(100)]
        public required string TimeZone { get; set; }

        public int Terminals { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        
    }
}
