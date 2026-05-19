using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Tests.Controllers
{
    public class AircraftConfigurationControllerTests
    {
        private readonly Mock<IAircraftConfigurationService> _mockService;
        private readonly AircraftConfigurationController _controller;

        public AircraftConfigurationControllerTests()
        {
            _mockService = new Mock<IAircraftConfigurationService>();
            _controller = new AircraftConfigurationController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var req = new CreateAircraftConfigurationRequest()
            {
                ConfigurationID = "1",
                AircraftModel = "ASA1",
            };
            var created = new AircraftConfigurationResponse { ConfigurationID = "1", AircraftModel = "ASA1" };
            _mockService.Setup(s => s.CreateAsync(req)).ReturnsAsync(created);
            var result = await _controller.Create(req);
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
