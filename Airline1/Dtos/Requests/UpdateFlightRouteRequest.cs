namespace Airline1.Dtos.Requests
{
    public class UpdateFlightRouteRequest
    {
        public required string Code { get; set; }
        public int? OriginAirportId { get; set; }
        public int? DestinationAirportId { get; set; }
        public double? DistanceKm { get; set; }
        public int? AverageFlightTimeMinutes { get; set; }
        public int? FrequencyWeekly { get; set; }
        public bool? IsActive { get; set; }
    }
}
