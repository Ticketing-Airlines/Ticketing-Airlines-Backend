using System;

namespace Airline1.Dtos.Responses
{
    public class AirportResponse
    {
        public int Id { get; set; }
        public required string IataCode { get; set; }
        public required string IcaoCode { get; set; }
        public required string Name { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public required string TimeZone { get; set; }
        public int Terminals { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
