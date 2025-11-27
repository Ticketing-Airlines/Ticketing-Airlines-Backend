using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;
using Airline1.Dtos.Requests;

namespace Airline1.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepo;
        private readonly Mock<IPassengerRepository> _mockPassengerRepo;
        private readonly AppDbContext _db;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _mockPassengerRepo = new Mock<IPassengerRepository>();
            var opt = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("bookingtest").Options;
            _db = new AppDbContext(opt);
            _service = new BookingService(_mockRepo.Object, _mockPassengerRepo.Object, _db);
        }

        [Fact]
        public async Task CreateBookingAsync_ReturnsNull_WhenFlightMissing()
        {
            var req = new CreateBookingRequest { FlightId = 999, Passengers = new List<CreatePassengerRequest>() };
            var res = await _service.CreateBookingAsync(req);
            Assert.Null(res);
        }
    }
}
