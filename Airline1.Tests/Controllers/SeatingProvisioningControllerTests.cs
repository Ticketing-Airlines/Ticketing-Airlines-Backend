using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Tests.Controllers
{
    public class SeatingProvisioningControllerTests
    {
        private readonly Mock<ISeatingProvisioningService> _mockService;
        private readonly SeatingProvisioningController _controller;

        public SeatingProvisioningControllerTests()
        {
            _mockService = new Mock<ISeatingProvisioningService>();
            _controller = new SeatingProvisioningController(_mockService.Object);
        }

        [Fact]
        public async Task ProvisionSeats_ReturnsOk()
        {
            _mockService.Setup(s => s.ProvisionSeatsForAircraftAsync(1));
            var result = await _controller.ProvisionSeats(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Seats provisioned", ok.Value.ToString());
        }

        [Fact]
        public async Task RegenerateSeats_ReturnsOk()
        {
            _mockService.Setup(s => s.RegenerateSeatsForAircraftAsync(1));
            var result = await _controller.RegenerateSeats(1);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Seats regenerated", ok.Value.ToString());
        }
    }
}
