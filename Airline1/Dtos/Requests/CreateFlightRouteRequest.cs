using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class CreateFlightRouteRequest
    {
        [MaxLength(50)]
        public required string Code { get; set; }
        public required int OriginAirportId { get; set; }
        public required int DestinationAirportId { get; set; }
        public double? DistanceKm { get; set; }
        public int? AverageFlightTimeMinutes { get; set; }
        public int FrequencyWeekly { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
