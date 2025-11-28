using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Tests.Controllers
{
    public class FlightStatusControllerTests
    {
        private readonly Mock<IFlightStatusService> _mockService;
        private readonly FlightStatusController _controller;

        public FlightStatusControllerTests()
        {
            _mockService = new Mock<IFlightStatusService>();
            _controller = new FlightStatusController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var req = new CreateFlightStatusRequest {FlightId = 1, Status = Common.FlightStatusType.Scheduled };
            var created = new { Id = 1 };
            _mockService.Setup(s => s.CreateAsync(req));
            var result = await _controller.Create(req);
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
