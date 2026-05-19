using Xunit;
using Microsoft.EntityFrameworkCore;
using Airline1.Services;
using Airline1.Data;
using Airline1.Models;

namespace Airline1.Tests.Services
{
    public class TrackingServiceTests
    {
        private readonly TrackingService _service;
        private readonly AppDbContext _context;

        public TrackingServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _service = new TrackingService(_context);
        }

        [Fact]
        public async Task GetLatestLocationAsync_ReturnsLocation_WhenDeviceExists()
        {
            // Arrange
            var deviceId = "device-001";
            var location = new DeviceLocation
            {
                DeviceId = deviceId,
                Latitude = 14.5995,
                Longitude = 120.9842,
                Timestamp = DateTime.UtcNow
            };
            _context.DeviceLocations.Add(location);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetLatestLocationAsync(deviceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deviceId, result.DeviceId);
            Assert.Equal(14.5995, result.Latitude);
            Assert.Equal(120.9842, result.Longitude);
        }

        [Fact]
        public async Task GetLatestLocationAsync_ReturnsNull_WhenDeviceNotFound()
        {
            // Arrange
            var deviceId = "nonexistent-device";

            // Act
            var result = await _service.GetLatestLocationAsync(deviceId);

            // Assert
            Assert.Null(result);
        }
    }
}