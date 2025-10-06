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
            new AirportResponse { Id = 1, Name = "Airport1" },
            new AirportResponse { Id = 2, Name = "Airport2" }
        };

        public static UpdateAirportRequest ValidUpdateRequest => new UpdateAirportRequest
        {
            Id = 1,
            Name = "Updated Airport"
        };

        public static UpdateAirportRequest InvalidUpdateRequest => new UpdateAirportRequest
        {
            Id = 99,
            Name = "Nonexistent"
        };
    }
}
