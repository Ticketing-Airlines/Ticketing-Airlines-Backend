using System;
using Airline1.Common;

namespace Airline1.Dtos.Responses
{
    public class AircraftResponse
    {
        public int Id { get; set; }
        public required string TailNumber { get; set; }
        public required string Manufacturer { get; set; }
        public required string Model { get; set; }
        public required string RegistrationNumber { get; set; }
        public required string ConfigurationID { get; set; }
        public DateTime? FirstFlightDate { get; set; }
        public AircraftType Type { get; set; }
        public int? BaseAirportId { get; set; }
        public string? BaseAirportName { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
