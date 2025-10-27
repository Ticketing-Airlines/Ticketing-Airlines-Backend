using System;
using System.ComponentModel.DataAnnotations;
using Airline1.Common;

namespace Airline1.Dtos.Requests
{
    public class UpdateAircraftRequest
    {
        // --- Core Identifiers are now OPTIONAL for partial updates (PATCH/PUT) ---
        [MaxLength(50)]
        public string? TailNumber { get; set; }

        [MaxLength(100)]
        public string? Manufacturer { get; set; }

        [MaxLength(100)]
        public string? Model { get; set; }

        [MaxLength(100)]
        public string? Nickname { get; set; }

        [MaxLength(100)]
        public string? RegistrationNumber { get; set; }

        // The client can update the configuration of the aircraft by changing this ID.
        [MaxLength(50)]
        public string? ConfigurationID { get; set; }

        public DateTime? FirstFlightDate { get; set; }

        // Made nullable to allow partial updates
        public AircraftType? Type { get; set; }

        // Optional Foreign Key
        public int? BaseAirportId { get; set; }
    }
}
