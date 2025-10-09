using Xunit;
using Moq;
using Airline1.Services.Implementations;
using Airline1.Repositories.Interfaces;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Airline1.Tests.TestData;

namespace Airline1.Tests.Services
{
    public class AirportServiceTests
    {
        private readonly Mock<IAirportRepository> _mockRepo;
        private readonly AirportService _service;

        public AirportServiceTests()
        {
            _mockRepo = new Mock<IAirportRepository>();
            // mock IMapper to avoid depending on AutoMapper in unit tests
            var mapperMock = new Mock<AutoMapper.IMapper>();
            mapperMock.Setup(m => m.Map<Airport>(It.IsAny<CreateAirportRequest>()))
                .Returns((CreateAirportRequest src) => new Airport {
                    IataCode = src.IataCode,
                    IcaoCode = src.IcaoCode,
                    Name = src.Name,
                    City = src.City,
                    Country = src.Country,
                    TimeZone = src.TimeZone
                });
            mapperMock.Setup(m => m.Map<AirportResponse>(It.IsAny<Airport>()))
                .Returns((Airport a) => new AirportResponse {
                    Id = a.Id,
                    IataCode = a.IataCode,
                    IcaoCode = a.IcaoCode,
                    Name = a.Name,
                    City = a.City,
                    Country = a.Country,
                    TimeZone = a.TimeZone
                });
            // Map from UpdateAirportRequest onto existing Airport (modify dest)
            mapperMock.Setup(m => m.Map(It.IsAny<UpdateAirportRequest>(), It.IsAny<Airport>()))
                .Callback((UpdateAirportRequest src, Airport dest) => {
                    if (src.IataCode != null) dest.IataCode = src.IataCode;
                    if (src.IcaoCode != null) dest.IcaoCode = src.IcaoCode;
                    if (src.Name != null) dest.Name = src.Name;
                    if (src.City != null) dest.City = src.City;
                    if (src.Country != null) dest.Country = src.Country;
                    if (src.TimeZone != null) dest.TimeZone = src.TimeZone;
                });

            _service = new AirportService(_mockRepo.Object, mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfAirports()
        {
            var airports = AirportServiceTestData.AirportList;
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(airports);
            var result = await _service.GetAllAsync();
            Assert.Equal(2, result.Count);
            Assert.Equal("Airport1", result[0].Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAirport_WhenExists()
        {
            var airport = AirportServiceTestData.Airport1;
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(airport);
            var result = await _service.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Airport)null);
            var result = await _service.GetByIdAsync(99);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedAirport()
        {
            var request = AirportServiceTestData.ValidCreateRequest;
            var airport = AirportServiceTestData.CreatedAirport;
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Airport>())).ReturnsAsync(airport);
            var result = await _service.CreateAsync(request);
            Assert.NotNull(result);
            Assert.Equal("New Airport", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsTrue_WhenAirportExists()
        {
            var request = AirportServiceTestData.ValidUpdateRequest;
            var airport = AirportServiceTestData.Airport1;
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(airport);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Airport>())).Returns(System.Threading.Tasks.Task.CompletedTask);
            var result = await _service.UpdateAsync(1, request);
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenAirportNotExists()
        {
            var request = AirportServiceTestData.InvalidUpdateRequest;
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Airport)null);
            var result = await _service.UpdateAsync(99, request);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenAirportExists()
        {
            var airport = AirportServiceTestData.Airport1;
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(airport);
            _mockRepo.Setup(r => r.DeleteAsync(It.IsAny<Airport>())).Returns(System.Threading.Tasks.Task.CompletedTask);
            var result = await _service.DeleteAsync(1);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenAirportNotExists()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Airport)null);
            var result = await _service.DeleteAsync(99);
            Assert.False(result);
        }
    }
}
