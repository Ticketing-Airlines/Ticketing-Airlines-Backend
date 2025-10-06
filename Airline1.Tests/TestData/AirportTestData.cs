using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System.Collections.Generic;

namespace Airline1.Tests.TestData
{
    public static class AirportTestData
    {
        public static CreateAirportRequest ValidCreateRequest => new CreateAirportRequest
        {
            IataCode = "iatacode",
            IcaoCode = "iacaocode",
            Name = "name",
            City = "city",
            Country = "country",
            TimeZone = "timezone"
        };

        public static AirportResponse ValidAirportResponse => new AirportResponse
        {
            Id = 1,
            IataCode = "iatacode",
            IcaoCode = "iacaocode",
            Name = "name",
            City = "city",
            Country = "country",
            TimeZone = "timezone"
        };

        public static List<AirportResponse> AirportList => new List<AirportResponse>
        {
            new AirportResponse {
                Id = 1,
                IataCode = "iata1",
                IcaoCode = "icao1",
                Name = "Airport1",
                City = "City1",
                Country = "Country1",
                TimeZone = "TZ1"
            },
            new AirportResponse {
                Id = 2,
                IataCode = "iata2",
                IcaoCode = "icao2",
                Name = "Airport2",
                City = "City2",
                Country = "Country2",
                TimeZone = "TZ2"
            }
        };

        public static UpdateAirportRequest ValidUpdateRequest => new UpdateAirportRequest
        {
            IataCode = "iata1",
            IcaoCode = "icao1",
            Name = "Updated Airport",
            City = "City1",
            Country = "Country1",
            TimeZone = "TZ1"
        };

        public static UpdateAirportRequest InvalidUpdateRequest => new UpdateAirportRequest
        {
            IataCode = "iata99",
            IcaoCode = "icao99",
            Name = "Nonexistent",
            City = "City99",
            Country = "Country99",
            TimeZone = "TZ99"
        };
    }
}
