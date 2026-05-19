using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Tests.Controllers
{
    public class TrackingControllerTests
    {
        private readonly Mock<ITrackingService> _mockService;
        private readonly TrackingController _controller;

        public TrackingControllerTests()
        {
            _mockService = new Mock<ITrackingService>();
            _controller = new TrackingController(_mockService.Object);
        }

        [Fact]
        public async Task GetLatestLocation_ReturnsOk_WhenFound()
        {
            var location = new DeviceLocationResponse
            {
                DeviceId = "device123",
                Latitude = 40.7128,
                Longitude = -74.0060
            };
            _mockService.Setup(s => s.GetLatestLocationAsync("device123")).ReturnsAsync(location);

            var result = await _controller.GetLatestLocation("device123");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetLatestLocation_ReturnsNotFound_WhenNotFound()
        {
            _mockService.Setup(s => s.GetLatestLocationAsync("unknown")).ReturnsAsync((DeviceLocationResponse?)null);

            var result = await _controller.GetLatestLocation("unknown");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFound.Value);
        }
    }
}