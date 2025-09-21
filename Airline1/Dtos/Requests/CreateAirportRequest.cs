using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class CreateAirportRequest
    {
        [Required, MaxLength(3)]
        public required string IataCode { get; set; }

        [MaxLength(4)]
        public required string IcaoCode { get; set; }

        [Required, MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(100)]
        public required string City { get; set; }

        [MaxLength(100)]
        public required string Country { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(100)]
        public required string TimeZone { get; set; }

        public int Terminals { get; set; } = 1;
    }
}
