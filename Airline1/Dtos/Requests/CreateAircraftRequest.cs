using System.ComponentModel.DataAnnotations;
using Airline1.Common;

namespace Airline1.Dtos.Requests
{
    public class CreateAircraftRequest
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

        [MaxLength(50)]
        public required string ConfigurationID { get; set; }

        public DateTime? FirstFlightDate { get; set; }

        public AircraftType Type { get; set; } = AircraftType.NarrowBody;

        public int? BaseAirportId { get; set; }
    }
}
