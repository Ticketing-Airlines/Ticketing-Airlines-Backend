using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class FlightRoute
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Code { get; set; }

        [Required]
        [ForeignKey(nameof(OriginAirport))]
        public int OriginAirportId { get; set; }
        public required Airport OriginAirport { get; set; }

        [Required]
        [ForeignKey(nameof(DestinationAirport))]
        public int DestinationAirportId { get; set; }
        public required Airport DestinationAirport { get; set; }

        public double? DistanceKm { get; set; }
        public int? AverageFlightTimeMinutes { get; set; }
        public int FrequencyWeekly { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
