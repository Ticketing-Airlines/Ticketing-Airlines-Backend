using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;
using Airline1.Dtos.Responses;

namespace Airline1.Tests.Controllers
{
    public class SeatSalesControllerTests
    {
        private readonly Mock<ISeatSaleService> _mockSeatSaleService;
        private readonly Mock<ISeatSaleConfigService> _mockConfigService;
        private readonly SeatSalesController _controller;

        public SeatSalesControllerTests()
        {
            _mockSeatSaleService = new Mock<ISeatSaleService>();
            _mockConfigService = new Mock<ISeatSaleConfigService>();
            _controller = new SeatSalesController(_mockSeatSaleService.Object, _mockConfigService.Object);
        }

        [Fact]
        public async Task GetActiveDeals_ReturnsOk_WhenDealsExist()
        {
            var deals = new SeatSaleActiveDealsResult
            {
                Deals = new List<SeatSaleResponse>
                {
                    new SeatSaleResponse
                    {
                        Id = "deal1",
                        Destination = "Paris",
                        DestinationAirportCode = "CDG",
                        Country = "France",
                        CountryIso2 = "FR",
                        Type = "EarlyBird",
                        Description = "Early bird special",
                        Image = "image.jpg",
                        Pricing = new SeatSalePricingResponse { Discount = 10, Currency = "USD" }
                    }
                },
                Total = 1
            };
            _mockSeatSaleService.Setup(s => s.GetActiveDealsAsync(null, null, null, null, null))
                .ReturnsAsync(deals);

            var result = await _controller.GetActiveDeals(null, null, null, null, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetActiveDeals_ReturnsNotFound_WhenKeyNotFoundException()
        {
            _mockSeatSaleService.Setup(s => s.GetActiveDealsAsync(null, null, null, null, null))
                .ThrowsAsync(new KeyNotFoundException("No active deals"));

            var result = await _controller.GetActiveDeals(null, null, null, null, null);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFound.Value);
        }

        [Fact]
        public async Task GetConfig_ReturnsOk_WhenConfigExists()
        {
            var config = new SeatSaleConfigResponse
            {
                SaleTitle = "Summer Sale",
                SaleSubtitle = "Save big",
                IsActive = true,
                HeroMessage = "Book now!"
            };
            _mockConfigService.Setup(s => s.GetConfigAsync()).ReturnsAsync(config);

            var result = await _controller.GetConfig();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetConfig_ReturnsNotFound_WhenKeyNotFoundException()
        {
            _mockConfigService.Setup(s => s.GetConfigAsync())
                .ThrowsAsync(new KeyNotFoundException("Config not found"));

            var result = await _controller.GetConfig();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFound.Value);
        }
    }
}