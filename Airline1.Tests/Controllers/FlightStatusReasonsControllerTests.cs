using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Tests.Controllers
{
    public class FlightStatusReasonsControllerTests
    {
        private readonly Mock<IFlightStatusReasonService> _mockService;
        private readonly FlightStatusReasonsController _controller;

        public FlightStatusReasonsControllerTests()
        {
            _mockService = new Mock<IFlightStatusReasonService>();
            _controller = new FlightStatusReasonsController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var req = new FlightStatusReasonCreateRequest {
                Code = "WX",
                Title = "Weather Delay",
                 };
            var created = new FlightStatusReasonResponse { Id = 1, Code = "WX", Title = "Weather Delay" };
            _mockService.Setup(s => s.CreateAsync(req)).ReturnsAsync(created);
            var result = await _controller.Create(req);
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
