using Xunit;
using Microsoft.Extensions.Caching.Memory;
using Airline1.Services;

namespace Airline1.Tests.Services
{
    public class CheckInRateLimiterTests
    {
        private readonly IMemoryCache _cache;
        private readonly CheckInRateLimiter _limiter;

        public CheckInRateLimiterTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _limiter = new CheckInRateLimiter(_cache);
        }

        [Fact]
        public void AllowComplete_ReturnsTrue_WhenFirstAttempt()
        {
            // Arrange
            var bookingRef = "ABC123";

            // Act
            var result = _limiter.AllowComplete(bookingRef);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AllowComplete_ReturnsTrue_WhenSecondAttempt()
        {
            // Arrange
            var bookingRef = "XYZ789";
            _limiter.AllowComplete(bookingRef);

            // Act
            var result = _limiter.AllowComplete(bookingRef);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AllowComplete_ReturnsFalse_WhenFourthAttempt()
        {
            // Arrange
            var bookingRef = "DEF456";
            _limiter.AllowComplete(bookingRef);
            _limiter.AllowComplete(bookingRef);
            _limiter.AllowComplete(bookingRef);

            // Act
            var result = _limiter.AllowComplete(bookingRef);

            // Assert
            Assert.False(result);
        }
    }
}