using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    public class SeatControllerTests
    {
        private readonly Mock<ISeatService> _mockService;
        private readonly SeatController _controller;

        public SeatControllerTests()
        {
            _mockService = new Mock<ISeatService>();
            _controller = new SeatController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var req = new CreateSeatRequest();
            var created = new { Id = 1 };
            _mockService.Setup(s => s.CreateAsync(req)).ReturnsAsync(created);
            var result = await _controller.Create(req);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task GetByAircraft_ReturnsOk()
        {
            var list = new List<object> { new { Id = 1 } };
            _mockService.Setup(s => s.GetByAircraftAsync(1)).ReturnsAsync(list);
            var result = await _controller.GetByAircraft(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }
    }
}
