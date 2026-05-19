using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Data;
using Airline1.Models;
using Airline1.Dtos.Requests;
using Airline1.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Airline1.Tests.Services
{
    public class CheckInServiceTests
    {
        private readonly Mock<ICheckInRepository> _mockCheckInRepo;
        private readonly Mock<IBookingRepository> _mockBookingRepo;
        private readonly AppDbContext _db;
        private readonly Mock<ILogger<CheckInService>> _mockLogger;
        private readonly CheckInService _service;

        public CheckInServiceTests()
        {
            _mockCheckInRepo = new Mock<ICheckInRepository>();
            _mockBookingRepo = new Mock<IBookingRepository>();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _db = new AppDbContext(options);
            _mockLogger = new Mock<ILogger<CheckInService>>();
            _service = new CheckInService(_mockCheckInRepo.Object, _mockBookingRepo.Object, _db, _mockLogger.Object);
        }

        [Fact]
        public async Task GetEligibilityAsync_ReturnsEligible_WhenWithinWindowAndConfirmed()
        {
            // Arrange
            var bookingRef = "ABC123";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var flight = booking.BookingFlights.First().Flight;
            flight.DepartureTime = DateTime.UtcNow.AddHours(12); // 12 hours from now - domestic check-in window

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);

            // Act
            var result = await _service.GetEligibilityAsync(bookingRef);

            // Assert
            Assert.True(result.IsEligible);
            Assert.Equal(bookingRef, result.BookingReference);
        }

        [Fact]
        public async Task GetEligibilityAsync_ReturnsNotEligible_WhenBookingNotFound()
        {
            // Arrange
            var bookingRef = "NOTFND";
            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync((Booking?)null);

            // Act
            var result = await _service.GetEligibilityAsync(bookingRef);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Equal("Booking not found", result.Message);
        }

        [Fact]
        public async Task GetEligibilityAsync_ReturnsNotEligible_WhenNoFlightInfo()
        {
            // Arrange
            var bookingRef = "NOFLIGHT";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            booking.BookingFlights.First().Flight = null!;

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);

            // Act
            var result = await _service.GetEligibilityAsync(bookingRef);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Equal("No flight information found", result.Message);
        }

        [Fact]
        public async Task GetEligibilityAsync_ReturnsNotEligible_WhenFlightCancelled()
        {
            // Arrange
            var bookingRef = "CANCELLD";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var flight = booking.BookingFlights.First().Flight;
            flight.DepartureTime = DateTime.UtcNow.AddHours(12);

            // Add cancelled status
            flight.Statuses = new List<FlightStatus>
            {
                new FlightStatus { Status = FlightStatusType.Cancelled, IsCurrent = true, FlightId = flight.Id }
            };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);

            // Act
            var result = await _service.GetEligibilityAsync(bookingRef);

            // Assert
            Assert.False(result.IsEligible);
            Assert.Contains("cancelled", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task VerifyAsync_ThrowsInvalidOperationException_WhenInvalidFormat()
        {
            // Arrange
            var request = new CheckInVerifyRequest { BookingReference = "INVALID!", LastName = "Smith" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.VerifyAsync(request));
            Assert.Equal("Invalid booking reference format", ex.Message);
        }

        [Fact]
        public async Task VerifyAsync_ReturnsNotVerified_WhenBookingNotFound()
        {
            // Arrange
            var request = new CheckInVerifyRequest { BookingReference = "NOTFND", LastName = "Smith" };
            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync("NOTFND")).ReturnsAsync((Booking?)null);

            // Act
            var result = await _service.VerifyAsync(request);

            // Assert
            Assert.False(result.IsVerified);
            Assert.Equal("BOOKING_NOT_FOUND", result.ErrorCode);
        }

        [Fact]
        public async Task VerifyAsync_ReturnsNotVerified_WhenWrongLastName()
        {
            // Arrange
            var bookingRef = "WRONGL";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var request = new CheckInVerifyRequest { BookingReference = bookingRef, LastName = "WrongLastName" };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);
            _mockCheckInRepo.Setup(r => r.GetCheckInsByBookingIdAsync(booking.BookingId)).ReturnsAsync(new List<CheckIn>());

            // Act
            var result = await _service.VerifyAsync(request);

            // Assert
            Assert.False(result.IsVerified);
            Assert.Equal("CHECK_IN_NOT_AVAILABLE", result.ErrorCode);
        }

        [Fact]
        public async Task VerifyAsync_ReturnsNotVerified_WhenAlreadyCheckedIn()
        {
            // Arrange
            var bookingRef = "ALRDYC";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var request = new CheckInVerifyRequest { BookingReference = bookingRef, LastName = "Doe" };

            var existingCheckIn = new CheckIn { BookingId = booking.BookingId, Status = "CheckedIn", PassengerId = booking.Passengers.First().BookingPassengerId };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);
            _mockCheckInRepo.Setup(r => r.GetCheckInsByBookingIdAsync(booking.BookingId)).ReturnsAsync(new List<CheckIn> { existingCheckIn });

            // Act
            var result = await _service.VerifyAsync(request);

            // Assert
            Assert.False(result.IsVerified);
            Assert.Equal("ALREADY_CHECKED_IN", result.ErrorCode);
        }

        [Fact]
        public async Task VerifyAsync_ReturnsVerified_WhenValidRequest()
        {
            // Arrange
            var bookingRef = "VALID1";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var flight = booking.BookingFlights.First().Flight;
            flight.DepartureTime = DateTime.UtcNow.AddHours(12);

            var request = new CheckInVerifyRequest { BookingReference = bookingRef, LastName = "Doe" };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);
            _mockCheckInRepo.Setup(r => r.GetCheckInsByBookingIdAsync(booking.BookingId)).ReturnsAsync(new List<CheckIn>());

            // Act
            var result = await _service.VerifyAsync(request);

            // Assert
            Assert.True(result.IsVerified);
            Assert.Null(result.ErrorCode);
        }

        [Fact]
        public async Task CompleteAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var bookingRef = "SUCCESS";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var flight = booking.BookingFlights.First().Flight;
            flight.DepartureTime = DateTime.UtcNow.AddHours(12);

            var passenger = booking.Passengers.First();
            var seatId = Guid.NewGuid();
            var seat = new Seat { Id = seatId, SeatNumber = "12A", AircraftId = 1 };
            var flightSeat = new FlightSeat { FlightSeatId = Guid.NewGuid(), FlightId = flight.Id, SeatId = seatId, SeatClass = "Economy", Status = "Available", Seat = seat };

            var request = new CheckInCompleteRequest
            {
                BookingReference = bookingRef,
                Passengers = new List<PassengerSeatAssignment>
                {
                    new PassengerSeatAssignment { PassengerId = passenger.BookingPassengerId, SeatNumber = "12A" }
                }
            };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);
            _mockCheckInRepo.Setup(r => r.GetFlightSeatByFlightAndSeatNumberAsync(flight.Id, "12A")).ReturnsAsync(flightSeat);
            _mockCheckInRepo.Setup(r => r.GetAvailableSeatsForFlightAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<FlightSeat>());
            _mockCheckInRepo.Setup(r => r.UpdateFlightSeatAsync(It.IsAny<FlightSeat>())).Returns(Task.CompletedTask);
            _mockCheckInRepo.Setup(r => r.AddCheckInAsync(It.IsAny<CheckIn>())).Returns(Task.CompletedTask);
            _mockCheckInRepo.Setup(r => r.AddBoardingPassAsync(It.IsAny<BoardingPass>())).Returns(Task.CompletedTask);
            _mockCheckInRepo.Setup(r => r.UpdateBookingAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);
            _mockCheckInRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CompleteAsync(request);

            // Assert
            Assert.Equal(bookingRef, result.BookingReference);
            Assert.Single(result.BoardingPasses);
        }

        [Fact]
        public async Task CompleteAsync_ThrowsKeyNotFoundException_WhenBookingNotFound()
        {
            // Arrange
            var request = new CheckInCompleteRequest
            {
                BookingReference = "NOTFND",
                Passengers = new List<PassengerSeatAssignment>()
            };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync("NOTFND")).ReturnsAsync((Booking?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CompleteAsync(request));
        }

        [Fact]
        public async Task CompleteAsync_ThrowsInvalidOperationException_WhenPassengerNotInBooking()
        {
            // Arrange
            var bookingRef = "MISSPASS";
            var booking = CreateTestBooking(bookingRef, "Confirmed");

            var request = new CheckInCompleteRequest
            {
                BookingReference = bookingRef,
                Passengers = new List<PassengerSeatAssignment>
                {
                    new PassengerSeatAssignment { PassengerId = Guid.NewGuid(), SeatNumber = "12A" }
                }
            };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteAsync(request));
            Assert.Contains("not part of this booking", ex.Message);
        }

        [Fact]
        public async Task CompleteAsync_ThrowsInvalidOperationException_WhenNoSeatAssigned()
        {
            // Arrange
            var bookingRef = "NOSEAT";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var passenger = booking.Passengers.First();

            var request = new CheckInCompleteRequest
            {
                BookingReference = bookingRef,
                Passengers = new List<PassengerSeatAssignment>
                {
                    new PassengerSeatAssignment { PassengerId = passenger.BookingPassengerId, SeatNumber = "" }
                }
            };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteAsync(request));
            Assert.Contains("must have a seat number", ex.Message);
        }

        [Fact]
        public async Task CompleteAsync_ThrowsInvalidOperationException_WhenSeatUnavailable()
        {
            // Arrange
            var bookingRef = "SEATUNAV";
            var booking = CreateTestBooking(bookingRef, "Confirmed");
            var flight = booking.BookingFlights.First().Flight;
            var passenger = booking.Passengers.First();

            var seatIdForUnavailableTest = Guid.NewGuid();
            var flightSeat = new FlightSeat { FlightSeatId = Guid.NewGuid(), FlightId = flight.Id, SeatId = seatIdForUnavailableTest, SeatClass = "Economy", Status = "Booked" };

            var request = new CheckInCompleteRequest
            {
                BookingReference = bookingRef,
                Passengers = new List<PassengerSeatAssignment>
                {
                    new PassengerSeatAssignment { PassengerId = passenger.BookingPassengerId, SeatNumber = "12A" }
                }
            };

            _mockCheckInRepo.Setup(r => r.GetBookingForCheckInAsync(bookingRef)).ReturnsAsync(booking);
            _mockCheckInRepo.Setup(r => r.GetFlightSeatByFlightAndSeatNumberAsync(flight.Id, "12A")).ReturnsAsync(flightSeat);
            _mockCheckInRepo.Setup(r => r.GetAvailableSeatsForFlightAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<FlightSeat>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CompleteAsync(request));
            Assert.StartsWith("SEAT_UNAVAILABLE", ex.Message);
        }

        private Booking CreateTestBooking(string pnr, string status)
        {
            var bookingId = Guid.NewGuid();
            var flightId = 1;
            var routeId = 1;

            var originAirport = new Airport { Id = 1, IataCode = "MNL", IcaoCode = "RPLL", Name = "Ninoy Aquino", City = "Manila", Country = "Philippines", TimeZone = "Asia/Manila" };
            var destinationAirport = new Airport { Id = 2, IataCode = "CEB", IcaoCode = "RPVM", Name = "Mactan-Cebu", City = "Cebu", Country = "Philippines", TimeZone = "Asia/Manila" };

            var route = new FlightRoute
            {
                Id = routeId,
                Code = "MNL-CEB",
                OriginAirportId = 1,
                DestinationAirportId = 2,
                OriginAirport = originAirport,
                DestinationAirport = destinationAirport
            };

            var aircraft = new Aircraft { Id = 1, TailNumber = "RP-C1234", Manufacturer = "Airbus", Model = "A320", RegistrationNumber = "ABC123", ConfigurationID = "CFG001" };

            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "AB123",
                AircraftId = 1,
                RouteId = routeId,
                DepartureTime = DateTime.UtcNow.AddHours(24),
                ArrivalTime = DateTime.UtcNow.AddHours(26),
                Route = route,
                Aircraft = aircraft,
                Statuses = new List<FlightStatus>()
            };

            var bookingFlight = new BookingFlight
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                FlightId = flightId,
                Flight = flight
            };

            var passengerId = Guid.NewGuid();
            var passenger = new BookingPassenger
            {
                BookingPassengerId = passengerId,
                BookingId = bookingId,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "M",
                PassengerType = "ADT"
            };

            var booking = new Booking
            {
                BookingId = bookingId,
                Pnr = pnr,
                FlightBundleId = 1,
                ContactEmail = "test@test.com",
                ContactPhone = "1234567890",
                TotalPrice = 1000m,
                Currency = "PHP",
                Status = status,
                BookingFlights = new List<BookingFlight> { bookingFlight },
                Passengers = new List<BookingPassenger> { passenger }
            };

            return booking;
        }
    }
}