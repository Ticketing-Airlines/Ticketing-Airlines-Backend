using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Tests.Controllers
{
    public class AddOnPriceControllerTests
    {
        private readonly Mock<IAddOnPriceService> _mockService;
        private readonly AddOnPriceController _controller;

        public AddOnPriceControllerTests()
        {
            _mockService = new Mock<IAddOnPriceService>();
            _controller = new AddOnPriceController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var req = new CreateAddOnPriceRequest()
            {
                FlightId = 1,
                AddOnId = 1,
                PriceAmount = 50.0m,
                Currency = "USD",
                ValidFrom = System.DateTime.UtcNow,
            };
            var created = new AddOnPriceResponse { Id = 1, FlightId = 1, AddOnId = 1, PriceAmount = 50.0m, Currency = "USD" };
            _mockService.Setup(s => s.CreateAsync(req)).ReturnsAsync(created);
            var result = await _controller.CreatePriceRule(req);
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
