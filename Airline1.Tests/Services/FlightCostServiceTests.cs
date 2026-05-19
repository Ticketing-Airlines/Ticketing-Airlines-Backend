using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IService;
using Airline1.Dtos.Responses;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class FlightCostServiceTests
    {
        private readonly Mock<IFlightPriceService> _mockFlightPriceService;
        private readonly Mock<IAddOnPriceService> _mockAddOnPriceService;
        private readonly FlightCostService _service;

        public FlightCostServiceTests()
        {
            _mockFlightPriceService = new Mock<IFlightPriceService>();
            _mockAddOnPriceService = new Mock<IAddOnPriceService>();
            _service = new FlightCostService(_mockFlightPriceService.Object, _mockAddOnPriceService.Object);
        }

        [Fact]
        public async Task CalculateCostAsync_ReturnsTotalWithAddOns()
        {
            // Arrange
            var flightId = 1;
            var cabinClass = "Economy";
            var flightBundleId = 1;
            var passengerType = "ADT";
            var addOnPriceIds = new List<int> { 1, 2 };

            var basePriceResponse = new FlightPriceResponse { Id = 1, FlightId = flightId, BasePrice = 500m, PassengerType = "ADT" };
            var addOnCost = 100m;

            _mockFlightPriceService.Setup(s => s.GetCurrentPriceAsync(flightId, cabinClass, flightBundleId, passengerType, It.IsAny<DateTime?>()))
                .ReturnsAsync(basePriceResponse);
            _mockAddOnPriceService.Setup(s => s.GetTotalCostByIdsAsync(addOnPriceIds))
                .ReturnsAsync(addOnCost);

            // Act
            var result = await _service.CalculateCostAsync(flightId, cabinClass, flightBundleId, passengerType, addOnPriceIds);

            // Assert
            Assert.Equal(600m, result);
        }

        [Fact]
        public async Task CalculateCostAsync_ReturnsBasePriceWithoutAddOns()
        {
            // Arrange
            var flightId = 1;
            var cabinClass = "Business";
            var flightBundleId = 2;
            var passengerType = "ADT";

            var basePriceResponse = new FlightPriceResponse { Id = 1, FlightId = flightId, BasePrice = 1000m, PassengerType = "ADT" };

            _mockFlightPriceService.Setup(s => s.GetCurrentPriceAsync(flightId, cabinClass, flightBundleId, passengerType, It.IsAny<DateTime?>()))
                .ReturnsAsync(basePriceResponse);
            _mockAddOnPriceService.Setup(s => s.GetTotalCostByIdsAsync(null))
                .ReturnsAsync(0m);

            // Act
            var result = await _service.CalculateCostAsync(flightId, cabinClass, flightBundleId, passengerType, null);

            // Assert
            Assert.Equal(1000m, result);
        }

        [Fact]
        public async Task CalculateCostAsync_ThrowsApplicationException_WhenNoBasePrice()
        {
            // Arrange
            var flightId = 999;
            var cabinClass = "Economy";
            var flightBundleId = 1;
            var passengerType = "ADT";

            _mockFlightPriceService.Setup(s => s.GetCurrentPriceAsync(flightId, cabinClass, flightBundleId, passengerType, It.IsAny<DateTime?>()))
                .ReturnsAsync((FlightPriceResponse?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.CalculateCostAsync(flightId, cabinClass, flightBundleId, passengerType, null));
            Assert.Contains("No active base price found", ex.Message);
        }
    }
}