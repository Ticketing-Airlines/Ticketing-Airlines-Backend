using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;
using Airline1.Dtos.Responses;

namespace Airline1.Tests.Controllers
{
    public class PaymentMethodsControllerTests
    {
        private readonly Mock<IPaymentMethodService> _mockService;
        private readonly PaymentMethodsController _controller;

        public PaymentMethodsControllerTests()
        {
            _mockService = new Mock<IPaymentMethodService>();
            _controller = new PaymentMethodsController(_mockService.Object);
        }

        [Fact]
        public async Task GetActiveMethods_ReturnsOk_WhenMethodsExist()
        {
            var resultDto = new ActivePaymentMethodsDto
            {
                PaymentMethods = new List<PaymentMethodResponse>
                {
                    new PaymentMethodResponse
                    {
                        Id = "card1",
                        Category = "Card",
                        Name = "Credit Card",
                        Description = "Pay with credit card",
                        ProcessingTime = "Instant",
                        Fee = new PaymentFeeDto { Type = "Percentage", Amount = 2.5m, Currency = "USD", DisplayText = "2.5%" },
                        Color = "#000000",
                        Icon = "credit-card",
                        Features = new List<string> { "Secure" },
                        Providers = new List<string> { "Visa", "Mastercard" },
                        Availability = new PaymentAvailabilityDto { IsAvailable = true }
                    }
                },
                Categories = new List<string> { "Card" },
                Total = 1,
                Metadata = new ActivePaymentMethodsMetadata { LastUpdated = DateTime.UtcNow, Version = "1.0" }
            };
            _mockService.Setup(s => s.GetActiveMethodsAsync(null, null)).ReturnsAsync(resultDto);

            var result = await _controller.GetActiveMethods(null, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetActiveMethods_ReturnsSuccessFalse_WhenNoMethods()
        {
            var emptyResult = new ActivePaymentMethodsDto
            {
                PaymentMethods = new List<PaymentMethodResponse>(),
                Categories = new List<string>(),
                Total = 0,
                Metadata = new ActivePaymentMethodsMetadata { LastUpdated = DateTime.UtcNow, Version = "1.0" }
            };
            _mockService.Setup(s => s.GetActiveMethodsAsync(null, null)).ReturnsAsync(emptyResult);

            var result = await _controller.GetActiveMethods(null, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetFAQs_ReturnsOk()
        {
            var faqs = new List<PaymentFAQResponse>
            {
                new PaymentFAQResponse
                {
                    Id = 1,
                    Category = "General",
                    Question = "How do I pay?",
                    Answer = "Use card",
                    Icon = "help",
                    Color = "#000000"
                }
            };
            _mockService.Setup(s => s.GetFAQsAsync()).ReturnsAsync(faqs);

            var result = await _controller.GetFAQs();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }
    }
}