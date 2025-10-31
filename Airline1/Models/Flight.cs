using Airline1.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }
        public required string FlightNumber { get; set; } = null!;
        public required int AircraftId { get; set; }
        public required int RouteId { get; set; }
        public required DateTime DepartureTime { get; set; }
        public required DateTime ArrivalTime { get; set; }

        [ForeignKey("AircraftId")]
        public Aircraft? Aircraft { get; set; }

        [ForeignKey("RouteId")]
        public FlightRoute? Route { get; set; }

    }
}
