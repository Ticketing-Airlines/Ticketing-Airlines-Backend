using Airline1.Models;
using Airline1.Dtos.Requests;
using System.Collections.Generic;

namespace Airline1.Tests.TestData
{
    public static class AirportServiceTestData
    {
        public static Airport Airport1 => new Airport { Id = 1, Name = "Airport1" };
        public static Airport Airport2 => new Airport { Id = 2, Name = "Airport2" };
        public static List<Airport> AirportList => new List<Airport> { Airport1, Airport2 };

        public static CreateAirportRequest ValidCreateRequest => new CreateAirportRequest { Name = "New Airport" };
        public static Airport CreatedAirport => new Airport { Id = 2, Name = "New Airport" };

        public static UpdateAirportRequest ValidUpdateRequest => new UpdateAirportRequest { Id = 1, Name = "Updated" };
        public static UpdateAirportRequest InvalidUpdateRequest => new UpdateAirportRequest { Id = 99, Name = "Nonexistent" };
    }
}
