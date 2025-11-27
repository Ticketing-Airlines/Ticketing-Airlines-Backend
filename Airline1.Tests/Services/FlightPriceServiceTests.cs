using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using Airline1.Dtos.Requests;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class FlightPriceServiceTests
    {
        private readonly Mock<IFlightPriceRepository> _mockRepo;
        private readonly FlightPriceService _service;

        public FlightPriceServiceTests()
        {
            _mockRepo = new Mock<IFlightPriceRepository>();
            _service = new FlightPriceService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreated()
        {
            var req = new CreateFlightPriceRequest();
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<FlightPrice>())).ReturnsAsync(new FlightPrice { Id = 1 });
            var res = await _service.CreateAsync(req);
            Assert.Equal(1, res.Id);
        }
    }
}
