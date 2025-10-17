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

        [MaxLength(100)]
        public required string RegistrationNumber { get; set; }

        [Range(0, 2000)]
        public int SeatingCapacity { get; set; } = 0;

        public DateTime? FirstFlightDate { get; set; }

        public AircraftType Type { get; set; } = AircraftType.NarrowBody;

        public int? BaseAirportId { get; set; }

        public AircraftStatus Status { get; set; } = AircraftStatus.Active;
    }
}
