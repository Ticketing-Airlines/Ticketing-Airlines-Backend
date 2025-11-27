using Airline_Ticketing.Controllers;
using Airline_Ticketing.Data;
using Airline_Ticketing.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Airline_Tcketing_Tests.Controllers
{
    public class FlightsControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly FlightsController _controller;

        public FlightsControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new FlightsController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetAllFlights Tests

        [Fact]
        public async Task GetAllFlights_WhenFlightsExist_ReturnsOkWithFlights()
        {
            // Arrange
            var flights = new List<Flights>
            {
                new Flights
                {
                    FlightID = 1,
                    FlightNumber = 101,
                    AircraftID = 1,
                    OriginAirportID = 1,
                    DestinationAirportID = 2,
                    DepartureTime = new TimeOnly(10, 30),
                    ArrivalTime = new TimeOnly(14, 45),
                    Status = "Scheduled",
                    CreatedBy = "Admin"
                },
                new Flights
                {
                    FlightID = 2,
                    FlightNumber = 102,
                    AircraftID = 2,
                    OriginAirportID = 3,
                    DestinationAirportID = 4,
                    DepartureTime = new TimeOnly(08, 00),
                    ArrivalTime = new TimeOnly(11, 30),
                    Status = "Scheduled",
                    CreatedBy = "Admin"
                }
            };

            await _context.Flights.AddRangeAsync(flights);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllFlights();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFlights = Assert.IsAssignableFrom<List<Flights>>(okResult.Value);
            Assert.Equal(2, returnedFlights.Count);
            Assert.Equal(101, returnedFlights[0].FlightNumber);
            Assert.Equal(102, returnedFlights[1].FlightNumber);
        }

        [Fact]
        public async Task GetAllFlights_WhenNoFlightsExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            // No flights added to database

            // Act
            var result = await _controller.GetAllFlights();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFlights = Assert.IsAssignableFrom<List<Flights>>(okResult.Value);
            Assert.Empty(returnedFlights);
        }

        [Fact]
        public async Task GetAllFlights_ReturnsAllFlightsFromDatabase()
        {
            // Arrange
            var flights = new List<Flights>
            {
                new Flights
                {
                    FlightID = 1,
                    FlightNumber = 201,
                    AircraftID = 1,
                    OriginAirportID = 1,
                    DestinationAirportID = 2,
                    DepartureTime = new TimeOnly(06, 00),
                    ArrivalTime = new TimeOnly(09, 00),
                    Status = "On Time",
                    CreatedBy = "System"
                },
                new Flights
                {
                    FlightID = 2,
                    FlightNumber = 202,
                    AircraftID = 2,
                    OriginAirportID = 2,
                    DestinationAirportID = 3,
                    DepartureTime = new TimeOnly(12, 00),
                    ArrivalTime = new TimeOnly(15, 30),
                    Status = "Delayed",
                    CreatedBy = "System"
                },
                new Flights
                {
                    FlightID = 3,
                    FlightNumber = 203,
                    AircraftID = 3,
                    OriginAirportID = 3,
                    DestinationAirportID = 1,
                    DepartureTime = new TimeOnly(18, 00),
                    ArrivalTime = new TimeOnly(21, 00),
                    Status = "Cancelled",
                    CreatedBy = "System"
                }
            };

            await _context.Flights.AddRangeAsync(flights);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllFlights();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFlights = Assert.IsAssignableFrom<List<Flights>>(okResult.Value);
            Assert.Equal(3, returnedFlights.Count);
            Assert.Contains(returnedFlights, f => f.FlightNumber == 201);
            Assert.Contains(returnedFlights, f => f.FlightNumber == 202);
            Assert.Contains(returnedFlights, f => f.FlightNumber == 203);
        }

        #endregion

        #region GetFlightById Tests

        [Fact]
        public async Task GetFlightById_WhenFlightExists_ReturnsOkWithFlight()
        {
            // Arrange
            var flight = new Flights
            {
                FlightID = 1,
                FlightNumber = 101,
                AircraftID = 1,
                OriginAirportID = 1,
                DestinationAirportID = 2,
                DepartureTime = new TimeOnly(10, 30),
                ArrivalTime = new TimeOnly(14, 45),
                Status = "Scheduled",
                CreatedBy = "Admin"
            };

            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFlightById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFlight = Assert.IsType<Flights>(okResult.Value);
            Assert.Equal(1, returnedFlight.FlightID);
            Assert.Equal(101, returnedFlight.FlightNumber);
            Assert.Equal("Scheduled", returnedFlight.Status);
        }

        [Fact]
        public async Task GetFlightById_WhenFlightDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var result = await _controller.GetFlightById(nonExistentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetFlightById_WithValidId_ReturnsCorrectFlight()
        {
            // Arrange
            var flights = new List<Flights>
            {
                new Flights
                {
                    FlightID = 1,
                    FlightNumber = 301,
                    AircraftID = 1,
                    OriginAirportID = 1,
                    DestinationAirportID = 2,
                    DepartureTime = new TimeOnly(07, 00),
                    ArrivalTime = new TimeOnly(10, 00),
                    Status = "On Time",
                    CreatedBy = "Admin"
                },
                new Flights
                {
                    FlightID = 2,
                    FlightNumber = 302,
                    AircraftID = 2,
                    OriginAirportID = 3,
                    DestinationAirportID = 4,
                    DepartureTime = new TimeOnly(13, 00),
                    ArrivalTime = new TimeOnly(16, 00),
                    Status = "Delayed",
                    CreatedBy = "Admin"
                }
            };

            await _context.Flights.AddRangeAsync(flights);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFlightById(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFlight = Assert.IsType<Flights>(okResult.Value);
            Assert.Equal(2, returnedFlight.FlightID);
            Assert.Equal(302, returnedFlight.FlightNumber);
            Assert.Equal("Delayed", returnedFlight.Status);
            Assert.Equal(3, returnedFlight.OriginAirportID);
            Assert.Equal(4, returnedFlight.DestinationAirportID);
        }

        [Fact]
        public async Task GetFlightById_WithMultipleFlights_ReturnsOnlyRequestedFlight()
        {
            // Arrange
            var flights = new List<Flights>
            {
                new Flights { FlightID = 1, FlightNumber = 401, AircraftID = 1, OriginAirportID = 1, DestinationAirportID = 2, DepartureTime = new TimeOnly(08, 00), ArrivalTime = new TimeOnly(11, 00), Status = "Scheduled", CreatedBy = "Admin" },
                new Flights { FlightID = 2, FlightNumber = 402, AircraftID = 2, OriginAirportID = 2, DestinationAirportID = 3, DepartureTime = new TimeOnly(12, 00), ArrivalTime = new TimeOnly(15, 00), Status = "Scheduled", CreatedBy = "Admin" },
                new Flights { FlightID = 3, FlightNumber = 403, AircraftID = 3, OriginAirportID = 3, DestinationAirportID = 4, DepartureTime = new TimeOnly(16, 00), ArrivalTime = new TimeOnly(19, 00), Status = "Scheduled", CreatedBy = "Admin" }
            };

            await _context.Flights.AddRangeAsync(flights);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFlightById(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFlight = Assert.IsType<Flights>(okResult.Value);
            Assert.Equal(2, returnedFlight.FlightID);
            Assert.Equal(402, returnedFlight.FlightNumber);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task GetFlightById_WithInvalidId_ReturnsNotFound(int invalidId)
        {
            // Arrange
            var flight = new Flights
            {
                FlightID = 1,
                FlightNumber = 501,
                AircraftID = 1,
                OriginAirportID = 1,
                DestinationAirportID = 2,
                DepartureTime = new TimeOnly(10, 00),
                ArrivalTime = new TimeOnly(13, 00),
                Status = "Scheduled",
                CreatedBy = "Admin"
            };

            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFlightById(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetFlightById_VerifyAllFlightProperties_ReturnsCompleteFlightData()
        {
            // Arrange
            var flight = new Flights
            {
                FlightID = 10,
                FlightNumber = 601,
                AircraftID = 5,
                OriginAirportID = 10,
                DestinationAirportID = 20,
                DepartureTime = new TimeOnly(14, 30),
                ArrivalTime = new TimeOnly(18, 45),
                Status = "On Time",
                CreatedBy = "TestAdmin"
            };

            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFlightById(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFlight = Assert.IsType<Flights>(okResult.Value);

            Assert.Equal(10, returnedFlight.FlightID);
            Assert.Equal(601, returnedFlight.FlightNumber);
            Assert.Equal(5, returnedFlight.AircraftID);
            Assert.Equal(10, returnedFlight.OriginAirportID);
            Assert.Equal(20, returnedFlight.DestinationAirportID);
            Assert.Equal(new TimeOnly(14, 30), returnedFlight.DepartureTime);
            Assert.Equal(new TimeOnly(18, 45), returnedFlight.ArrivalTime);
            Assert.Equal("On Time", returnedFlight.Status);
            Assert.Equal("TestAdmin", returnedFlight.CreatedBy);
        }

        #endregion
    }
}
