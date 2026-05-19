using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Data;
using Airline1.Models;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Tests.Services
{
    public class FlightStatusServiceTests
    {
        private readonly Mock<IFlightStatusRepository> _mockRepo;
        private readonly AppDbContext _db;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FlightStatusService _service;

        public FlightStatusServiceTests()
        {
            _mockRepo = new Mock<IFlightStatusRepository>();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new AppDbContext(options);
            _mockMapper = new Mock<IMapper>();
            _service = new FlightStatusService(_mockRepo.Object, _db, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var request = new CreateFlightStatusRequest
            {
                FlightId = 1,
                Status = FlightStatusType.Scheduled
            };

            var flight = new Flight { Id = 1, FlightNumber = "AB123", AircraftId = 1, RouteId = 1, DepartureTime = DateTime.UtcNow, ArrivalTime = DateTime.UtcNow.AddHours(2) };
            _db.Flights.Add(flight);
            await _db.SaveChangesAsync();

            var entity = new FlightStatus { Id = 1, FlightId = 1, Status = FlightStatusType.Scheduled };
            var response = new FlightStatusResponse { Id = 1, FlightId = 1, Status = FlightStatusType.Scheduled };

            _mockRepo.Setup(r => r.GetLatestByFlightIdAsync(1)).ReturnsAsync((FlightStatus?)null);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<FlightStatus>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FlightStatusResponse>(entity)).Returns(response);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FlightStatusType.Scheduled, result.Status);
        }

        [Fact]
        public async Task CreateAsync_ThrowsKeyNotFoundException_WhenFlightNotFound()
        {
            // Arrange
            var request = new CreateFlightStatusRequest
            {
                FlightId = 999,
                Status = FlightStatusType.Scheduled
            };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(request));
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenInvalidTransition()
        {
            // Arrange - flight with last status Cancelled, trying to set to Scheduled
            var flight = new Flight { Id = 2, FlightNumber = "XY789", AircraftId = 1, RouteId = 1, DepartureTime = DateTime.UtcNow, ArrivalTime = DateTime.UtcNow.AddHours(2) };
            _db.Flights.Add(flight);

            var lastStatus = new FlightStatus { Id = 1, FlightId = 2, Status = FlightStatusType.Cancelled };
            _db.FlightStatuses.Add(lastStatus);
            await _db.SaveChangesAsync();

            var request = new CreateFlightStatusRequest
            {
                FlightId = 2,
                Status = FlightStatusType.Scheduled
            };

            _mockRepo.Setup(r => r.GetLatestByFlightIdAsync(2)).ReturnsAsync(lastStatus);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.Contains("final status", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_ThrowsInvalidOperationException_WhenFinalStatusExists()
        {
            // Arrange - flight with last status Landed (final)
            var flight = new Flight { Id = 3, FlightNumber = "FINAL01", AircraftId = 1, RouteId = 1, DepartureTime = DateTime.UtcNow, ArrivalTime = DateTime.UtcNow.AddHours(2) };
            _db.Flights.Add(flight);

            var lastStatus = new FlightStatus { Id = 2, FlightId = 3, Status = FlightStatusType.Landed };
            _db.FlightStatuses.Add(lastStatus);
            await _db.SaveChangesAsync();

            var request = new CreateFlightStatusRequest
            {
                FlightId = 3,
                Status = FlightStatusType.Delayed
            };

            _mockRepo.Setup(r => r.GetLatestByFlightIdAsync(3)).ReturnsAsync(lastStatus);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.Contains("final status", ex.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsResponse_WhenFound()
        {
            // Arrange
            var entity = new FlightStatus { Id = 1, FlightId = 1, Status = FlightStatusType.Scheduled };
            var response = new FlightStatusResponse { Id = 1, FlightId = 1, Status = FlightStatusType.Scheduled };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FlightStatusResponse>(entity)).Returns(response);

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
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightStatus?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLatestByFlightIdAsync_ReturnsResponse_WhenFound()
        {
            // Arrange
            var entity = new FlightStatus { Id = 1, FlightId = 5, Status = FlightStatusType.Boarding };
            var response = new FlightStatusResponse { Id = 1, FlightId = 5, Status = FlightStatusType.Boarding };

            _mockRepo.Setup(r => r.GetLatestByFlightIdAsync(5)).ReturnsAsync(entity);
            _mockMapper.Setup(m => m.Map<FlightStatusResponse>(entity)).Returns(response);

            // Act
            var result = await _service.GetLatestByFlightIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FlightStatusType.Boarding, result.Status);
        }

        [Fact]
        public async Task GetLatestByFlightIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetLatestByFlightIdAsync(999)).ReturnsAsync((FlightStatus?)null);

            // Act
            var result = await _service.GetLatestByFlightIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetHistoryByFlightIdAsync_ReturnsList()
        {
            // Arrange
            var entities = new List<FlightStatus>
            {
                new FlightStatus { Id = 1, FlightId = 1, Status = FlightStatusType.Scheduled },
                new FlightStatus { Id = 2, FlightId = 1, Status = FlightStatusType.Boarding }
            };

            var responses = entities.Select(e => new FlightStatusResponse { Id = e.Id, FlightId = e.FlightId, Status = e.Status }).ToList();

            _mockRepo.Setup(r => r.GetHistoryByFlightIdAsync(1, 100)).ReturnsAsync(entities);
            _mockMapper.Setup(m => m.Map<FlightStatusResponse>(It.IsAny<FlightStatus>()))
                .Returns((FlightStatus fs) => new FlightStatusResponse { Id = fs.Id, FlightId = fs.FlightId, Status = fs.Status });

            // Act
            var result = await _service.GetHistoryByFlightIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ReturnsResponse_WhenSuccess()
        {
            // Arrange
            var existing = new FlightStatus { Id = 1, FlightId = 10, Status = FlightStatusType.Scheduled };
            var newEntity = new FlightStatus { Id = 2, FlightId = 10, Status = FlightStatusType.Boarding };
            var response = new FlightStatusResponse { Id = 2, FlightId = 10, Status = FlightStatusType.Boarding };

            var request = new UpdateFlightStatusRequest { Status = FlightStatusType.Boarding };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.GetLatestByFlightIdAsync(10)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<FlightStatus>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(newEntity);
            _mockMapper.Setup(m => m.Map<FlightStatusResponse>(It.IsAny<FlightStatus>())).Returns(response);

            // Act
            var result = await _service.UpdateAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(FlightStatusType.Boarding, result.Status);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var request = new UpdateFlightStatusRequest { Status = FlightStatusType.Boarding };

            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FlightStatus?)null);

            // Act
            var result = await _service.UpdateAsync(999, request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsInvalidOperationException_WhenFinalStatus()
        {
            // Arrange
            var existing = new FlightStatus { Id = 1, FlightId = 10, Status = FlightStatusType.Landed };
            var request = new UpdateFlightStatusRequest { Status = FlightStatusType.Delayed };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(1, request));
            Assert.Contains("final status", ex.Message);
        }
    }
}