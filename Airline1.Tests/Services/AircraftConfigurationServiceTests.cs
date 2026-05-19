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
    public class AircraftConfigurationServiceTests
    {
        private readonly Mock<IAircraftConfigurationRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AircraftConfigurationService _service;

        public AircraftConfigurationServiceTests()
        {
            _mockRepo = new Mock<IAircraftConfigurationRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new AircraftConfigurationService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arrange
            var configs = new List<AircraftConfiguration>
            {
                new AircraftConfiguration { ConfigurationID = "CFG001", AircraftModel = "A320", TotalSeats = 180 },
                new AircraftConfiguration { ConfigurationID = "CFG002", AircraftModel = "B737", TotalSeats = 160 }
            };

            var responses = configs.Select(c => new AircraftConfigurationResponse { ConfigurationID = c.ConfigurationID, AircraftModel = c.AircraftModel, TotalSeats = c.TotalSeats }).ToList();

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(configs);
            _mockMapper.Setup(m => m.Map<IEnumerable<AircraftConfigurationResponse>>(configs)).Returns(responses);

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
            var config = new AircraftConfiguration { ConfigurationID = "CFG001", AircraftModel = "A320", TotalSeats = 180 };
            var response = new AircraftConfigurationResponse { ConfigurationID = "CFG001", AircraftModel = "A320", TotalSeats = 180 };

            _mockRepo.Setup(r => r.GetByIdAsync("CFG001")).ReturnsAsync(config);
            _mockMapper.Setup(m => m.Map<AircraftConfigurationResponse>(config)).Returns(response);

            // Act
            var result = await _service.GetByIdAsync("CFG001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CFG001", result.ConfigurationID);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync("NOTFOUND")).ReturnsAsync((AircraftConfiguration?)null);

            // Act
            var result = await _service.GetByIdAsync("NOTFOUND");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var request = new CreateAircraftConfigurationRequest
            {
                ConfigurationID = "CFG003",
                AircraftModel = "A321",
                TotalSeats = 220,
                CabinDetails = new List<CabinDetailDto>
                {
                    new CabinDetailDto { CabinName = "Business", StartRow = 1, EndRow = 5, SeatMapLayout = "2-2" },
                    new CabinDetailDto { CabinName = "Economy", StartRow = 6, EndRow = 30, SeatMapLayout = "3-3" }
                }
            };

            var entity = new AircraftConfiguration { ConfigurationID = "CFG003", AircraftModel = "A321", TotalSeats = 220 };
            var response = new AircraftConfigurationResponse { ConfigurationID = "CFG003", AircraftModel = "A321", TotalSeats = 220 };

            _mockMapper.Setup(m => m.Map<AircraftConfiguration>(request)).Returns(entity);
            _mockRepo.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<AircraftConfigurationResponse>(entity)).Returns(response);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CFG003", result.ConfigurationID);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsTrue_WhenSuccess()
        {
            // Arrange
            var existing = new AircraftConfiguration { ConfigurationID = "CFG001", AircraftModel = "A320", TotalSeats = 180 };
            var request = new UpdateAircraftConfigurationRequest { AircraftModel = "A320neo", TotalSeats = 185 };

            _mockRepo.Setup(r => r.GetByIdAsync("CFG001")).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map(request, existing)).Returns(existing);

            // Act
            var result = await _service.UpdateAsync("CFG001", request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            var request = new UpdateAircraftConfigurationRequest { AircraftModel = "A320neo" };

            _mockRepo.Setup(r => r.GetByIdAsync("NOTFOUND")).ReturnsAsync((AircraftConfiguration?)null);

            // Act
            var result = await _service.UpdateAsync("NOTFOUND", request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenSuccess()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync("CFG001")).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync("CFG001");

            // Assert
            Assert.True(result);
        }
    }
}