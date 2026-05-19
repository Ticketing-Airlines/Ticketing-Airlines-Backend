using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using AutoMapper;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class FlightBundleServiceTests
    {
        private readonly Mock<IFlightBundleRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FlightBundleService _service;

        public FlightBundleServiceTests()
        {
            _mockRepo = new Mock<IFlightBundleRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new FlightBundleService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arrange
            var bundles = new List<FlightBundle>
            {
                new FlightBundle { Id = 1, Name = "Basic", Code = "BASIC", PriceIncrement = 0m },
                new FlightBundle { Id = 2, Name = "Premium", Code = "PREMIUM", PriceIncrement = 50m }
            };

            var responses = bundles.Select(b => new FlightBundleResponse { Id = b.Id, Name = b.Name, Code = b.Code }).ToList();

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(bundles);
            _mockMapper.Setup(m => m.Map<IEnumerable<FlightBundleResponse>>(bundles)).Returns(responses);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsResponse_WhenFound()
        {
            // Arrange
            var bundle = new FlightBundle { Id = 1, Name = "Basic", Code = "BASIC", PriceIncrement = 0m };
            var response = new FlightBundleResponse { Id = 1, Name = "Basic", Code = "BASIC", PriceIncrement = 0m };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bundle);
            _mockMapper.Setup(m => m.Map<FlightBundleResponse>(bundle)).Returns(response);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightBundle?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var request = new CreateFlightBundleRequest
            {
                Name = "New Bundle",
                Code = "NEW",
                PriceIncrement = 50m,
                CarryOnWeightKg = 7,
                CheckedBaggagePcs = 1,
                CheckedBaggageWeightKg = 23
            };

            var entity = new FlightBundle { Id = 1, Name = "New Bundle", Code = "NEW", PriceIncrement = 50m };
            var response = new FlightBundleResponse { Id = 1, Name = "New Bundle", Code = "NEW", PriceIncrement = 50m };

            _mockMapper.Setup(m => m.Map<FlightBundle>(request)).Returns(entity);
            _mockRepo.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<FlightBundleResponse>(entity)).Returns(response);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NEW", result.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var bundle = new FlightBundle { Id = 1, Name = "Updated", Code = "UPD", PriceIncrement = 25m };
            var request = new UpdateFlightBundleRequest { Name = "Updated", Code = "UPD" };
            var response = new FlightBundleResponse { Id = 1, Name = "Updated", Code = "UPD", PriceIncrement = 25m };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bundle);
            _mockRepo.Setup(r => r.UpdateAsync(bundle)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map(request, bundle)).Returns(bundle);
            _mockMapper.Setup(m => m.Map<FlightBundleResponse>(bundle)).Returns(response);

            // Act
            var result = await _service.UpdateAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UPD", result.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var request = new UpdateFlightBundleRequest { Name = "Updated", Code = "UPD" };

            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightBundle?)null);

            // Act
            var result = await _service.UpdateAsync(999, request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenSuccess()
        {
            // Arrange
            var bundle = new FlightBundle { Id = 1, Name = "ToDelete", Code = "DEL", PriceIncrement = 0m };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(bundle);
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightBundle?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}