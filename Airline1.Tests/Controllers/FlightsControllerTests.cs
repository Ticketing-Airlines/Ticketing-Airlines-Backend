using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    public class FlightsControllerTests
    {
        private readonly Mock<IFlightService> _mockService;
        private readonly FlightsController _controller;

        public FlightsControllerTests()
        {
            _mockService = new Mock<IFlightService>();
            _controller = new FlightsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var list = new List<object> { new { Id = 1 } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(list);
            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((object?)null);
            var result = await _controller.GetById(99);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
