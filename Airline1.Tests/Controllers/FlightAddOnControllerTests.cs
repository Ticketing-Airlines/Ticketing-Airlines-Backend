using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    public class FlightAddOnControllerTests
    {
        private readonly Mock<IFlightAddOnService> _mockService;
        private readonly FlightAddOnController _controller;

        public FlightAddOnControllerTests()
        {
            _mockService = new Mock<IFlightAddOnService>();
            _controller = new FlightAddOnController(_mockService.Object);
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
        public async Task GetByCategory_ReturnsOk()
        {
            var list = new List<object> { new { Id = 1 } };
            _mockService.Setup(s => s.GetByCategoryAsync(AddOnCategory.Food)).ReturnsAsync(list);
            var result = await _controller.GetByCategory(AddOnCategory.Food);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }
    }
}
