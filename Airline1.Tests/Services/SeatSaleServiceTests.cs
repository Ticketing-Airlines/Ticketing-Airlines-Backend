using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using AutoMapper;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class SeatSaleServiceTests
    {
        private readonly Mock<ISeatSaleRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SeatSaleService _service;

        public SeatSaleServiceTests()
        {
            _mockRepo = new Mock<ISeatSaleRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new SeatSaleService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetActiveDealsAsync_ReturnsResult_WhenDealsAvailable()
        {
            // Arrange
            var deals = new List<SeatSale>
            {
                new SeatSale
                {
                    Id = Guid.NewGuid(),
                    Destination = "Cebu",
                    DestinationAirportCode = "CEB",
                    Country = "Philippines",
                    CountryIso2 = "PH",
                    Type = "Domestic",
                    Description = "Test sale",
                    OriginalPrice = 5000m,
                    SalePrice = 3500m,
                    Currency = "PHP",
                    Features = "[]",
                    IsActive = true
                }
            };
            _mockRepo.Setup(r => r.GetActiveDealsAsync(null, null, null, null, null))
                .ReturnsAsync(deals);

            // Act
            var result = await _service.GetActiveDealsAsync(null, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Total);
            Assert.Single(result.Deals);
        }

        [Fact]
        public async Task GetActiveDealsAsync_ThrowsKeyNotFoundException_WhenNoDealsAvailable()
        {
            // Arrange
            var deals = new List<SeatSale>();
            _mockRepo.Setup(r => r.GetActiveDealsAsync(null, null, null, null, null))
                .ReturnsAsync(deals);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetActiveDealsAsync(null, null, null, null, null));
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSeatSale_WhenFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var seatSale = new SeatSale
            {
                Id = id,
                Destination = "Manila",
                DestinationAirportCode = "MNL",
                Country = "Philippines",
                CountryIso2 = "PH",
                Type = "Domestic",
                Description = "Test sale",
                OriginalPrice = 8000m,
                SalePrice = 5500m,
                Currency = "PHP",
                Features = "[]",
                IsActive = true
            };
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(seatSale);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MNL", result.DestinationAirportCode);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((SeatSale?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            Assert.Null(result);
        }
    }
}