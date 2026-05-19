using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Tests.Services
{
    public class SeatSaleConfigServiceTests
    {
        private readonly Mock<ISeatSaleConfigRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SeatSaleConfigService _service;

        public SeatSaleConfigServiceTests()
        {
            _mockRepo = new Mock<ISeatSaleConfigRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new SeatSaleConfigService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetConfigAsync_ReturnsConfig_WhenFound()
        {
            // Arrange
            var config = new SeatSaleConfig
            {
                Id = 1,
                SaleEndDate = DateTime.UtcNow.AddDays(30),
                SaleTitle = "Summer Sale",
                SaleSubtitle = "Book now and save",
                IsActive = true,
                HeroMessage = "Up to 50% off",
                TermsAndConditions = new List<TermsCondition>(),
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow
            };
            _mockRepo.Setup(r => r.GetConfigAsync()).ReturnsAsync(config);

            // Act
            var result = await _service.GetConfigAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Summer Sale", result.SaleTitle);
            Assert.Equal("Book now and save", result.SaleSubtitle);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetConfigAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetConfigAsync()).ReturnsAsync((SeatSaleConfig?)null);

            // Act
            var result = await _service.GetConfigAsync();

            // Assert
            Assert.Null(result);
        }
    }
}