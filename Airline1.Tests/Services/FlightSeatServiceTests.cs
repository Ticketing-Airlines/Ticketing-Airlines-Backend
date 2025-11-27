using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class FlightSeatServiceTests
    {
        private readonly Mock<IFlightSeatRepository> _mockRepo;
        private readonly FlightSeatService _service;

        public FlightSeatServiceTests()
        {
            _mockRepo = new Mock<IFlightSeatRepository>();
            _service = new FlightSeatService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetByFlightAsync_ReturnsList()
        {
            var list = new List<FlightSeat> { new FlightSeat { Id = 1 } };
            _mockRepo.Setup(r => r.GetByFlightAsync(1)).ReturnsAsync(list);
            var res = await _service.GetByFlightAsync(1);
            Assert.Single(res);
        }
    }
}
