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
    public class FlightStatusReasonServiceTests
    {
        private readonly Mock<IFlightStatusReasonRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FlightStatusReasonService _service;

        public FlightStatusReasonServiceTests()
        {
            _mockRepo = new Mock<IFlightStatusReasonRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new FlightStatusReasonService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arrange
            var entities = new List<FlightStatusReason>
            {
                new FlightStatusReason { Id = 1, Code = "WX", Title = "Weather" },
                new FlightStatusReason { Id = 2, Code = "TECH", Title = "Technical" }
            };

            var responses = entities.Select(e => new FlightStatusReasonResponse { Id = e.Id, Code = e.Code, Title = e.Title }).ToList();

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
            _mockMapper.Setup(m => m.Map<FlightStatusReasonResponse>(It.IsAny<FlightStatusReason>()))
                .Returns((FlightStatusReason r) => new FlightStatusReasonResponse { Id = r.Id, Code = r.Code, Title = r.Title });

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
            var entity = new FlightStatusReason { Id = 1, Code = "WX", Title = "Weather" };
            var response = new FlightStatusReasonResponse { Id = 1, Code = "WX", Title = "Weather" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FlightStatusReasonResponse>(entity)).Returns(response);

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
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightStatusReason?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByCodeAsync_ReturnsResponse_WhenFound()
        {
            // Arrange
            var entity = new FlightStatusReason { Id = 1, Code = "WX", Title = "Weather" };
            var response = new FlightStatusReasonResponse { Id = 1, Code = "WX", Title = "Weather" };

            _mockRepo.Setup(r => r.GetByCodeAsync("WX")).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FlightStatusReasonResponse>(entity)).Returns(response);

            // Act
            var result = await _service.GetByCodeAsync("WX");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WX", result.Code);
        }

        [Fact]
        public async Task GetByCodeAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByCodeAsync("NOTFOUND")).ReturnsAsync((FlightStatusReason?)null);

            // Act
            var result = await _service.GetByCodeAsync("NOTFOUND");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var request = new FlightStatusReasonCreateRequest { Code = "NEW", Title = "New Reason" };
            var entity = new FlightStatusReason { Id = 1, Code = "NEW", Title = "New Reason" };
            var response = new FlightStatusReasonResponse { Id = 1, Code = "NEW", Title = "New Reason" };

            _mockRepo.Setup(r => r.GetByCodeAsync("NEW")).ReturnsAsync((FlightStatusReason?)null);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<FlightStatusReason>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<FlightStatusReasonResponse>(It.IsAny<FlightStatusReason>())).Returns(response);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NEW", result.Code);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenDuplicateCode()
        {
            // Arrange
            var request = new FlightStatusReasonCreateRequest { Code = "DUP", Title = "Duplicate" };
            var existing = new FlightStatusReason { Id = 1, Code = "DUP", Title = "Existing" };

            _mockRepo.Setup(r => r.GetByCodeAsync("DUP")).ReturnsAsync(existing);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var entity = new FlightStatusReason { Id = 1, Code = "UPD", Title = "Updated Title" };
            var request = new FlightStatusReasonUpdateRequest { Title = "Updated Title" };
            var response = new FlightStatusReasonResponse { Id = 1, Code = "UPD", Title = "Updated Title" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mockRepo.Setup(r => r.Update(entity));
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<FlightStatusReasonResponse>(entity)).Returns(response);

            // Act
            var result = await _service.UpdateAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Title", result.Title);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var request = new FlightStatusReasonUpdateRequest { Title = "New Title" };

            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightStatusReason?)null);

            // Act
            var result = await _service.UpdateAsync(999, request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenSuccess()
        {
            // Arrange
            var entity = new FlightStatusReason { Id = 1, Code = "DEL", Title = "To Delete" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mockRepo.Setup(r => r.Remove(entity));
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
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightStatusReason?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}