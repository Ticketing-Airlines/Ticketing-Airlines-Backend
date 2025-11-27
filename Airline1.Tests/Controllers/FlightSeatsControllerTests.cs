using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    public class FlightSeatsControllerTests
    {
        private readonly Mock<IFlightSeatService> _mockService;
        private readonly FlightSeatsController _controller;

        public FlightSeatsControllerTests()
        {
            _mockService = new Mock<IFlightSeatService>();
            _controller = new FlightSeatsController(_mockService.Object);
        }

        [Fact]
        public async Task GetByFlight_ReturnsOk()
        {
            var list = new List<object> { new { Id = 1 } };
            _mockService.Setup(s => s.GetByFlightAsync(1)).ReturnsAsync(list);
            var result = await _controller.GetByFlight(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }
    }
}
