
using Airline1.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class Aircraft
    {
        [Key]
        public int Id { get; set; }

        // e.g. "RP-C1234" or tail number. Should be unique.
        [Required, MaxLength(50)]
        public required string TailNumber { get; set; }

        [MaxLength(100)]
        public required string Manufacturer { get; set; }    // e.g. Airbus, Boeing

        [MaxLength(100)]
        public required string Model { get; set; }           // e.g. A320, B737-800

        [MaxLength(100)]
        public required string RegistrationNumber { get; set; } // official registration

        public int SeatingCapacity { get; set; }    // number of seats

        public DateTime? FirstFlightDate { get; set; }

        public AircraftType Type { get; set; } = AircraftType.NarrowBody;

        public AircraftStatus Status { get; set; } = AircraftStatus.Active;

        // optional: where the aircraft is based or currently located
        [ForeignKey(nameof(BaseAirport))]
        public int? BaseAirportId { get; set; }
        public required Airport BaseAirport { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
