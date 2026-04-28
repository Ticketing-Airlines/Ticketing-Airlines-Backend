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

        // ✅ Each Flight can have multiple pricing records
        public ICollection<FlightPrice>? FlightPrices { get; set; }

        // ✅ Each Flight can have multiple statuses
        public ICollection<FlightStatus>? Statuses { get; set; }

        public int? AirlineId { get; set; }

        [ForeignKey(nameof(AirlineId))]
        public Airline? Airline { get; set; }

        public int? RescheduledFromFlightId { get; set; }
        [ForeignKey(nameof(RescheduledFromFlightId))]
        public Flight? RescheduledFromFlight { get; set; }

        // 🟢 FIX: Renamed property to 'Seats' to match the AppDbContext configuration:
        // b.HasOne(fs => fs.Flight).WithMany(f => f.Seats)
        public ICollection<FlightSeat>? Seats { get; set; }

        // Flight status lookup fields
        [MaxLength(10)]
        public string? DepartureTerminal { get; set; }
        [MaxLength(10)]
        public string? DepartureGate { get; set; }
        [MaxLength(10)]
        public string? ArrivalTerminal { get; set; }
        [MaxLength(10)]
        public string? ArrivalGate { get; set; }
        public DateTime? ActualDepartureTime { get; set; }
        public DateTime? EstimatedArrivalTime { get; set; }
        public DateTime? ActualArrivalTime { get; set; }
        public int? DelayMinutes { get; set; }
    }
}