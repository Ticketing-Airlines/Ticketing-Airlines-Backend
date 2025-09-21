using System;

namespace Airline1.Dtos.Responses
{
    public class FlightRouteResponse
    {
        public int Id { get; set; }
        public required string Code { get; set; }

        public int OriginAirportId { get; set; }
        public required string OriginAirportName { get; set; }

        public int DestinationAirportId { get; set; }
        public required string DestinationAirportName { get; set; }

        public double? DistanceKm { get; set; }
        public int? AverageFlightTimeMinutes { get; set; }
        public int FrequencyWeekly { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
