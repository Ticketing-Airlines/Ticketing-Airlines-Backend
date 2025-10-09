using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Airline1.Services.Implementations;
using Airline1.Repositories.Interfaces;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Models;
using AutoMapper;
using System;

namespace Airline1.Tests.Services
{
    public class FlightRouteServiceTests
    {
        private readonly Mock<IFlightRouteRepository> _mockRepo;
        private readonly Mock<IAirportRepository> _mockAirportRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FlightRouteService _service;

        public FlightRouteServiceTests()
        {
            _mockRepo = new Mock<IFlightRouteRepository>();
            _mockAirportRepo = new Mock<IAirportRepository>();
            _mockMapper = new Mock<IMapper>();

            // Basic mapper behaviour for create/map to model and back to response
            _mockMapper.Setup(m => m.Map<FlightRoute>(It.IsAny<CreateFlightRouteRequest>())).Returns((CreateFlightRouteRequest r) => new FlightRoute {
                Code = r.Code,
                OriginAirportId = r.OriginAirportId,
                DestinationAirportId = r.DestinationAirportId,
                DistanceKm = r.DistanceKm ?? 0,
                AverageFlightTimeMinutes = r.AverageFlightTimeMinutes,
                FrequencyWeekly = r.FrequencyWeekly,
                IsActive = r.IsActive
            });

            _mockMapper.Setup(m => m.Map<FlightRouteResponse>(It.IsAny<FlightRoute>())).Returns((FlightRoute fr) => new FlightRouteResponse {
                Id = fr.Id,
                Code = fr.Code,
                OriginAirportId = fr.OriginAirportId,
                OriginAirportName = "Origin",
                DestinationAirportId = fr.DestinationAirportId,
                DestinationAirportName = "Dest",
                DistanceKm = fr.DistanceKm,
                AverageFlightTimeMinutes = fr.AverageFlightTimeMinutes,
                FrequencyWeekly = fr.FrequencyWeekly,
                IsActive = fr.IsActive,
                CreatedAt = fr.CreatedAt,
                UpdatedAt = fr.UpdatedAt
            });

            // Map UpdateRequest onto FlightRoute (modify dest)
            _mockMapper.Setup(m => m.Map(It.IsAny<UpdateFlightRouteRequest>(), It.IsAny<FlightRoute>()))
                .Callback((UpdateFlightRouteRequest src, FlightRoute dest) => {
                    if (src.Code != null) dest.Code = src.Code;
                    if (src.OriginAirportId.HasValue) dest.OriginAirportId = src.OriginAirportId.Value;
                    if (src.DestinationAirportId.HasValue) dest.DestinationAirportId = src.DestinationAirportId.Value;
                    if (src.DistanceKm.HasValue) dest.DistanceKm = src.DistanceKm.Value;
                    if (src.AverageFlightTimeMinutes.HasValue) dest.AverageFlightTimeMinutes = src.AverageFlightTimeMinutes.Value;
                    if (src.FrequencyWeekly.HasValue) dest.FrequencyWeekly = src.FrequencyWeekly.Value;
                    if (src.IsActive.HasValue) dest.IsActive = src.IsActive.Value;
                });

            _service = new FlightRouteService(_mockRepo.Object, _mockAirportRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_CreatesRoute_WhenAirportsExistAndNoDuplicate()
        {
            var req = new CreateFlightRouteRequest { Code = "R1", OriginAirportId = 1, DestinationAirportId = 2 };

            _mockAirportRepo.Setup(a => a.ExistsAsync(1)).ReturnsAsync(true);
            _mockAirportRepo.Setup(a => a.ExistsAsync(2)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.GetByOriginDestinationAsync(1, 2)).ReturnsAsync((FlightRoute)null);

            var saved = new FlightRoute { Id = 10, Code = "R1", OriginAirportId = 1, DestinationAirportId = 2, CreatedAt = DateTime.UtcNow };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<FlightRoute>())).ReturnsAsync(saved);

            var res = await _service.CreateAsync(req);
            Assert.NotNull(res);
            Assert.Equal(10, res.Id);
            Assert.Equal("R1", res.Code);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenOriginMissing()
        {
            var req = new CreateFlightRouteRequest { Code = "R1", OriginAirportId = 999, DestinationAirportId = 2 };
            _mockAirportRepo.Setup(a => a.ExistsAsync(999)).ReturnsAsync(false);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(req));
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenDuplicateExists()
        {
            var req = new CreateFlightRouteRequest { Code = "R1", OriginAirportId = 1, DestinationAirportId = 2 };
            _mockAirportRepo.Setup(a => a.ExistsAsync(1)).ReturnsAsync(true);
            _mockAirportRepo.Setup(a => a.ExistsAsync(2)).ReturnsAsync(true);
            var duplicate = new FlightRoute { Id = 5, Code = "R1", OriginAirportId = 1, DestinationAirportId = 2 };
            _mockRepo.Setup(r => r.GetByOriginDestinationAsync(1, 2)).ReturnsAsync(duplicate);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(req));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedList()
        {
            var items = new List<FlightRoute> {
                new FlightRoute { Id = 1, Code = "R1", OriginAirportId = 1, DestinationAirportId = 2 },
                new FlightRoute { Id = 2, Code = "R2", OriginAirportId = 3, DestinationAirportId = 4 }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
            var res = await _service.GetAllAsync();
            Assert.Equal(2, res.Count);
            Assert.Equal("R1", res[0].Code);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FlightRoute)null);
            var res = await _service.GetByIdAsync(99);
            Assert.Null(res);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMapped_WhenFound()
        {
            var item = new FlightRoute { Id = 2, Code = "R2", OriginAirportId = 3, DestinationAirportId = 4, CreatedAt = DateTime.UtcNow };
            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            var res = await _service.GetByIdAsync(2);
            Assert.NotNull(res);
            Assert.Equal(2, res.Id);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FlightRoute)null);
            var req = new UpdateFlightRouteRequest { Code = "X" };
            var res = await _service.UpdateAsync(99, req);
            Assert.Null(res);
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenNewAirportsInvalid()
        {
            var existing = new FlightRoute { Id = 5, Code = "R5", OriginAirportId = 1, DestinationAirportId = 2 };
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);
            var req = new UpdateFlightRouteRequest { OriginAirportId = 999 };
            _mockAirportRepo.Setup(a => a.ExistsAsync(999)).ReturnsAsync(false);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(5, req));
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenDuplicateFound()
        {
            var existing = new FlightRoute { Id = 5, Code = "R5", OriginAirportId = 1, DestinationAirportId = 3 };
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);
            var req = new UpdateFlightRouteRequest { OriginAirportId = 1, DestinationAirportId = 2 };
            var duplicate = new FlightRoute { Id = 6, OriginAirportId = 1, DestinationAirportId = 2 };
            _mockAirportRepo.Setup(a => a.ExistsAsync(1)).ReturnsAsync(true);
            _mockAirportRepo.Setup(a => a.ExistsAsync(2)).ReturnsAsync(true);
            _mockRepo.Setup(r => r.GetByOriginDestinationAsync(1, 2)).ReturnsAsync(duplicate);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(5, req));
        }

        [Fact]
        public async Task UpdateAsync_Succeeds()
        {
            var existing = new FlightRoute { Id = 5, Code = "R5", OriginAirportId = 1, DestinationAirportId = 3, FrequencyWeekly = 2 };
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);
            _mockAirportRepo.Setup(a => a.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            var req = new UpdateFlightRouteRequest { FrequencyWeekly = 8 };
            _mockRepo.Setup(r => r.GetByOriginDestinationAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((FlightRoute)null);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<FlightRoute>())).Returns(Task.CompletedTask);
            var res = await _service.UpdateAsync(5, req);
            Assert.NotNull(res);
            Assert.Equal(8, res.FrequencyWeekly);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenExists()
        {
            var existing = new FlightRoute { Id = 7, Code = "R7" };
            _mockRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);
            var res = await _service.DeleteAsync(7);
            Assert.True(res);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FlightRoute)null);
            var res = await _service.DeleteAsync(99);
            Assert.False(res);
        }
    }
}
