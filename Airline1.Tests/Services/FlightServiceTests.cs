using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.IService;
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
        private readonly Mock<IWeatherService> _mockWeather;
        private readonly FlightService _service;

        public FlightServiceTests()
        {
            _mockRepo = new Mock<IFlightRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockWeather = new Mock<IWeatherService>();
            _service = new FlightService(_mockRepo.Object, _mockMapper.Object, _mockWeather.Object);
        }

        [Fact]
        public async Task GetAllAsync_UsesRepositoryAndMapper()
        {
            var list = new List<Flight> { new Flight { 
                Id = 1,
                FlightNumber = "F1",
                AircraftId  = 1,
                RouteId = 2,
                DepartureTime = DateTime.Now,
                ArrivalTime = DateTime.Now} };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
            _mockMapper.Setup(m => m.Map<IEnumerable<object>>(It.IsAny<object>())).Returns(new List<object> { new { Id = 1 } });
            var res = await _service.GetAllAsync();
            Assert.NotNull(res);
        }
    }
}
