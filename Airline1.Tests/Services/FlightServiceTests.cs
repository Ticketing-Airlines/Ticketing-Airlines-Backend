using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using AutoMapper;
using Airline1.Models;
using Airline1.Dtos.Requests;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class FlightServiceTests
    {
        private readonly Mock<IFlightRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FlightService _service;

        public FlightServiceTests()
        {
            _mockRepo = new Mock<IFlightRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new FlightService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_UsesRepositoryAndMapper()
        {
            var list = new List<Flight> { new Flight { Id = 1, FlightNumber = "F1" } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
            _mockMapper.Setup(m => m.Map<IEnumerable<object>>(It.IsAny<object>())).Returns(new List<object> { new { Id = 1 } });
            var res = await _service.GetAllAsync();
            Assert.NotNull(res);
        }
    }
}
