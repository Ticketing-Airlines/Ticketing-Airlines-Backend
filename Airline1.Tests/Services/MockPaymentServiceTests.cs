using Xunit;
using Airline1.Services;

namespace Airline1.Tests.Services
{
    public class MockPaymentServiceTests
    {
        private readonly MockPaymentService _service;

        public MockPaymentServiceTests()
        {
            _service = new MockPaymentService();
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnsSuccessCode_ForGCash()
        {
            // Arrange
            var amount = 1500.00m;
            var currency = "PHP";
            var paymentMethod = "GCash";
            var reference = "BOOKING123";

            // Act
            var result = await _service.ProcessPaymentAsync(amount, currency, paymentMethod, reference);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("MOCK-GCASH-", result);
            Assert.Contains(reference, result);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ThrowsInvalidOperationException_ForUnsupportedMethod()
        {
            // Arrange
            var amount = 1500.00m;
            var currency = "PHP";
            var paymentMethod = "Bitcoin";
            var reference = "BOOKING456";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.ProcessPaymentAsync(amount, currency, paymentMethod, reference));
        }
    }
}