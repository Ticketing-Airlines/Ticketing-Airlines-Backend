using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Services;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Airline1.Tests.Controllers
{
    public class CheckInControllerTests
    {
        private readonly Mock<ICheckInService> _mockService;
        private readonly CheckInRateLimiter _rateLimiter;
        private readonly IMemoryCache _cache;
        private readonly CheckInController _controller;

        public CheckInControllerTests()
        {
            _mockService = new Mock<ICheckInService>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _rateLimiter = new CheckInRateLimiter(_cache);
            _controller = new CheckInController(_mockService.Object, _rateLimiter);
        }

        [Fact]
        public async Task GetEligibility_ReturnsOk()
        {
            var response = new CheckInEligibilityResponse
            {
                BookingReference = "ABC123",
                IsEligible = true,
                Message = "Eligible"
            };
            _mockService.Setup(s => s.GetEligibilityAsync("ABC123")).ReturnsAsync(response);

            var result = await _controller.GetEligibility("ABC123");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Verify_ReturnsOk_WhenVerified()
        {
            var request = new CheckInVerifyRequest { BookingReference = "ABC123", LastName = "Doe" };
            var response = new CheckInVerifyResponse
            {
                BookingReference = "ABC123",
                IsVerified = true
            };
            _mockService.Setup(s => s.VerifyAsync(request)).ReturnsAsync(response);

            var result = await _controller.Verify(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Verify_ReturnsBadRequest_WhenNotVerified()
        {
            var request = new CheckInVerifyRequest { BookingReference = "ABC123", LastName = "Wrong" };
            var response = new CheckInVerifyResponse
            {
                BookingReference = "ABC123",
                IsVerified = false
            };
            _mockService.Setup(s => s.VerifyAsync(request)).ReturnsAsync(response);

            var result = await _controller.Verify(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task Complete_ReturnsOk_WhenSuccessful()
        {
            var request = new CheckInCompleteRequest
            {
                BookingReference = "ABC123",
                Passengers = new List<PassengerSeatAssignment>()
            };
            var response = new CheckInCompleteResponse
            {
                BookingReference = "ABC123",
                Status = "CheckedIn",
                CheckedInAt = DateTime.UtcNow
            };
            _mockService.Setup(s => s.CompleteAsync(request)).ReturnsAsync(response);

            var result = await _controller.Complete(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task Complete_ReturnsStatusCode429_WhenRateLimited()
        {
            var request = new CheckInCompleteRequest
            {
                BookingReference = "RATELTD",
                Passengers = new List<PassengerSeatAssignment>()
            };

            // Exhaust rate limit (3 attempts)
            await _controller.Complete(request);
            await _controller.Complete(request);
            await _controller.Complete(request);

            // 4th attempt should be rate limited
            var result = await _controller.Complete(request);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(429, statusResult.StatusCode);
        }

        [Fact]
        public async Task Complete_ReturnsNotFound_WhenKeyNotFoundException()
        {
            var request = new CheckInCompleteRequest
            {
                BookingReference = "NOTFOUND",
                Passengers = new List<PassengerSeatAssignment>()
            };
            _mockService.Setup(s => s.CompleteAsync(request))
                .ThrowsAsync(new KeyNotFoundException("Booking not found"));

            var result = await _controller.Complete(request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFound.Value);
        }

        [Fact]
        public async Task Complete_ReturnsConflict_WhenSeatUnavailable()
        {
            var request = new CheckInCompleteRequest
            {
                BookingReference = "NOSEATS",
                Passengers = new List<PassengerSeatAssignment>()
            };
            _mockService.Setup(s => s.CompleteAsync(request))
                .ThrowsAsync(new InvalidOperationException("SEAT_UNAVAILABLE:1A,2A"));

            var result = await _controller.Complete(request);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(conflict.Value);
        }

        [Fact]
        public async Task Complete_ReturnsBadRequest_WhenInvalidOperationException()
        {
            var request = new CheckInCompleteRequest
            {
                BookingReference = "INVALID",
                Passengers = new List<PassengerSeatAssignment>()
            };
            _mockService.Setup(s => s.CompleteAsync(request))
                .ThrowsAsync(new InvalidOperationException("Some other error"));

            var result = await _controller.Complete(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }
    }
}