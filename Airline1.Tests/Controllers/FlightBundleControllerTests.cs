using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    public class FlightBundleControllerTests
    {
        private readonly Mock<IFlightBundleService> _mockService;
        private readonly FlightBundleController _controller;

        public FlightBundleControllerTests()
        {
            _mockService = new Mock<IFlightBundleService>();
            _controller = new FlightBundleController(_mockService.Object);
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
        public async Task Create_ReturnsCreated()
        {
            var req = new CreateFlightBundleRequest();
            var created = new { Id = 1 };
            _mockService.Setup(s => s.CreateAsync(req)).ReturnsAsync(created);
            var result = await _controller.Create(req);
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
