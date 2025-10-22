using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FlightNumber { get; set; } = null!;

        [Required]
        public int AircraftId { get; set; }

        [Required]
        public int RouteId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public decimal Price { get; set; }

        [ForeignKey("AircraftId")]
        public Aircraft? Aircraft { get; set; }

        [ForeignKey("RouteId")]
        public FlightRoute? Route { get; set; }
    }
}
