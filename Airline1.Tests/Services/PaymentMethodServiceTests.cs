using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class PaymentMethodServiceTests
    {
        private readonly Mock<IPaymentMethodRepository> _mockMethodRepo;
        private readonly Mock<IPaymentFAQRepository> _mockFaqRepo;
        private readonly PaymentMethodService _service;

        public PaymentMethodServiceTests()
        {
            _mockMethodRepo = new Mock<IPaymentMethodRepository>();
            _mockFaqRepo = new Mock<IPaymentFAQRepository>();
            _service = new PaymentMethodService(_mockMethodRepo.Object, _mockFaqRepo.Object);
        }

        [Fact]
        public async Task GetActiveMethodsAsync_ReturnsAllActiveMethods()
        {
            // Arrange
            var methods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = "gcash", Name = "GCash", Category = "E-Wallet", Description = "GCash payment", ProcessingTime = "Instant", FeeType = "Percentage", FeeAmount = 0m, FeeCurrency = "PHP", FeeDisplayText = "Free", Color = "green", Icon = "gcash_icon", IsActive = true, Featured = true },
                new PaymentMethod { Id = "paymaya", Name = "PayMaya", Category = "E-Wallet", Description = "PayMaya payment", ProcessingTime = "Instant", FeeType = "Percentage", FeeAmount = 0m, FeeCurrency = "PHP", FeeDisplayText = "Free", Color = "blue", Icon = "paymaya_icon", IsActive = true, Featured = false }
            };
            _mockMethodRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(methods);

            // Act
            var result = await _service.GetActiveMethodsAsync(null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Total);
            Assert.Contains("All", result.Categories);
            Assert.Contains("E-Wallet", result.Categories);
        }

        [Fact]
        public async Task GetActiveMethodsAsync_ReturnsMethodsByCategory()
        {
            // Arrange
            var methods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = "visa", Name = "Visa", Category = "Credit Card", Description = "Visa card", ProcessingTime = "3 days", FeeType = "Percentage", FeeAmount = 2.5m, FeeCurrency = "PHP", FeeDisplayText = "2.5%", Color = "navy", Icon = "visa_icon", IsActive = true, Featured = false }
            };
            _mockMethodRepo.Setup(r => r.GetActiveByCategoryAsync("Credit Card")).ReturnsAsync(methods);

            // Act
            var result = await _service.GetActiveMethodsAsync("Credit Card", null);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PaymentMethods);
            Assert.Equal("Visa", result.PaymentMethods[0].Name);
        }

        [Fact]
        public async Task GetActiveMethodsAsync_ReturnsFeaturedMethods()
        {
            // Arrange
            var methods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = "gcash", Name = "GCash", Category = "E-Wallet", Description = "GCash payment", ProcessingTime = "Instant", FeeType = "Percentage", FeeAmount = 0m, FeeCurrency = "PHP", FeeDisplayText = "Free", Color = "green", Icon = "gcash_icon", IsActive = true, Featured = true }
            };
            _mockMethodRepo.Setup(r => r.GetActiveFeaturedAsync()).ReturnsAsync(methods);

            // Act
            var result = await _service.GetActiveMethodsAsync(null, true);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.PaymentMethods);
            Assert.True(result.PaymentMethods[0].Featured);
        }

        [Fact]
        public async Task GetFAQsAsync_ReturnsAllActiveFAQs()
        {
            // Arrange
            var faqs = new List<PaymentFAQ>
            {
                new PaymentFAQ { Id = 1, Category = "General", Question = "How do I pay?", Answer = "Use GCash or PayMaya", Icon = "help", Color = "#FF0000", DisplayOrder = 1 },
                new PaymentFAQ { Id = 2, Category = "General", Question = "Is it safe?", Answer = "Yes, very safe", Icon = "shield", Color = "#00FF00", DisplayOrder = 2 }
            };
            _mockFaqRepo.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(faqs);

            // Act
            var result = await _service.GetFAQsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("How do I pay?", result[0].Question);
        }
    }
}