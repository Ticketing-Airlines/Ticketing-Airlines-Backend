using Airline1.Models;
using Airline1.Dtos.Requests;
using System.Collections.Generic;

namespace Airline1.Tests.TestData
{
    public static class AirportServiceTestData
    {
        public static Airport Airport1 => new Airport {
            Id = 1,
            IataCode = "iata1",
            IcaoCode = "icao1",
            Name = "Airport1",
            City = "City1",
            Country = "Country1",
            TimeZone = "TZ1"
        };
        public static Airport Airport2 => new Airport {
            Id = 2,
            IataCode = "iata2",
            IcaoCode = "icao2",
            Name = "Airport2",
            City = "City2",
            Country = "Country2",
            TimeZone = "TZ2"
        };
        public static List<Airport> AirportList => new List<Airport> { Airport1, Airport2 };

        public static CreateAirportRequest ValidCreateRequest => new CreateAirportRequest {
            IataCode = "iata2",
            IcaoCode = "icao2",
            Name = "New Airport",
            City = "City2",
            Country = "Country2",
            TimeZone = "TZ2"
        };
        public static Airport CreatedAirport => new Airport {
            Id = 2,
            IataCode = "iata2",
            IcaoCode = "icao2",
            Name = "New Airport",
            City = "City2",
            Country = "Country2",
            TimeZone = "TZ2"
        };

        public static UpdateAirportRequest ValidUpdateRequest => new UpdateAirportRequest {
            IataCode = "iata1",
            IcaoCode = "icao1",
            Name = "Updated",
            City = "City1",
            Country = "Country1",
            TimeZone = "TZ1"
        };
        public static UpdateAirportRequest InvalidUpdateRequest => new UpdateAirportRequest {
            IataCode = "iata99",
            IcaoCode = "icao99",
            Name = "Nonexistent",
            City = "City99",
            Country = "Country99",
            TimeZone = "TZ99"
        };
    }
}
