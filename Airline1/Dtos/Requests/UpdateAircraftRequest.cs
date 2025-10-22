using System;
using System.ComponentModel.DataAnnotations;
using Airline1.Common;

namespace Airline1.Dtos.Requests
{
    public class UpdateAircraftRequest
    {
        [MaxLength(50)]
        public required string TailNumber { get; set; }

        [MaxLength(100)]
        public required string Manufacturer { get; set; }

        [MaxLength(100)]
        public required string Model { get; set; }

        public string? Nickname { get; set; }

        [MaxLength(100)]
        public required string RegistrationNumber { get; set; }

        public int? SeatingCapacity { get; set; }

        public DateTime? FirstFlightDate { get; set; }

        public AircraftType? Type { get; set; }

        public int? BaseAirportId { get; set; }

        public AircraftStatus? Status { get; set; }
    }
}
