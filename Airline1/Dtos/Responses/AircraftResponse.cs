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
        public int SeatingCapacity { get; set; }
        public DateTime? FirstFlightDate { get; set; }
        public AircraftType Type { get; set; }
        public AircraftStatus Status { get; set; }
        public int? BaseAirportId { get; set; }
        public required string BaseAirportName { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
