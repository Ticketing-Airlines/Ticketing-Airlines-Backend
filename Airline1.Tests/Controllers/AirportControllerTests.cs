using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.Services.Interfaces;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Airline1.Tests.TestData;

namespace Airline1.Tests.Controllers
{
    public class AirportsControllerTests
    {
        private readonly Mock<IAirportService> _mockService;
        private readonly AirportsController _controller;

        public AirportsControllerTests()
        {
            _mockService = new Mock<IAirportService>();
            _controller = new AirportsController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenValid()
        {
            var request = AirportTestData.ValidCreateRequest;
            var response = AirportTestData.ValidAirportResponse;
            _mockService.Setup(x => x.CreateAsync(request)).ReturnsAsync(response);
            var result = await _controller.Create(request);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(response, createdResult.Value);
        }

            [Fact]
            public async Task GetAll_ReturnsOkResult_WithListOfAirports()
            {
                var airports = new List<AirportResponse> {
                    new AirportResponse {
                        Id = 1,
                        IataCode = "iata1",
                        IcaoCode = "icao1",
                        Name = "Airport1",
                        City = "City1",
                        Country = "Country1",
                        TimeZone = "TZ1"
                    },
                    new AirportResponse {
                        Id = 2,
                        IataCode = "iata2",
                        IcaoCode = "icao2",
                        Name = "Airport2",
                        City = "City2",
                        Country = "Country2",
                        TimeZone = "TZ2"
                    }
                };
                _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(airports);
                var result = await _controller.GetAll();
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnValue = Assert.IsType<List<AirportResponse>>(okResult.Value);
                Assert.Equal(2, returnValue.Count);
            }

            [Fact]
            public async Task GetById_ReturnsOkResult_WhenAirportExists()
            {
                var airport = new AirportResponse {
                    Id = 1,
                    IataCode = "iata1",
                    IcaoCode = "icao1",
                    Name = "Airport1",
                    City = "City1",
                    Country = "Country1",
                    TimeZone = "TZ1"
                };
                _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(airport);
                var result = await _controller.GetById(1);
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnValue = Assert.IsType<AirportResponse>(okResult.Value);
                Assert.Equal(1, returnValue.Id);
            }

            [Fact]
            public async Task GetById_ReturnsNotFound_WhenAirportDoesNotExist()
            {
                _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((AirportResponse?)null);
                var result = await _controller.GetById(99);
                Assert.IsType<NotFoundResult>(result.Result);
            }

            [Fact]
            public async Task Update_ReturnsNoContent_WhenUpdateIsSuccessful()
            {
                var request = new UpdateAirportRequest {
                    IataCode = "iata1",
                    IcaoCode = "icao1",
                    Name = "Updated Airport",
                    City = "City1",
                    Country = "Country1",
                    TimeZone = "TZ1"
                };
                var updatedResponse = new AirportResponse {
                    Id = 1,
                    IataCode = "iata1",
                    IcaoCode = "icao1",
                    Name = "Updated Airport",
                    City = "City1",
                    Country = "Country1",
                    TimeZone = "TZ1"
                };
                _mockService.Setup(s => s.UpdateAsync(1, request)).ReturnsAsync(updatedResponse);
                var result = await _controller.Update(1, request);
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnValue = Assert.IsType<AirportResponse>(okResult.Value);
                Assert.Equal("Updated Airport", returnValue.Name);
            }

            [Fact]
            public async Task Update_ReturnsNotFound_WhenAirportDoesNotExist()
            {
                var request = new UpdateAirportRequest {
                    IataCode = "iata99",
                    IcaoCode = "icao99",
                    Name = "Nonexistent",
                    City = "City99",
                    Country = "Country99",
                    TimeZone = "TZ99"
                };
                _mockService.Setup(s => s.UpdateAsync(99, request)).ReturnsAsync((AirportResponse?)null);
                var result = await _controller.Update(99, request);
                Assert.IsType<NotFoundResult>(result);
            }

            [Fact]
            public async Task Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
            {
                _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
                var result = await _controller.Delete(1);
                Assert.IsType<NoContentResult>(result);
            }

            [Fact]
            public async Task Delete_ReturnsNotFound_WhenAirportDoesNotExist()
            {
                _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);
                var result = await _controller.Delete(99);
                Assert.IsType<NotFoundResult>(result);
            }
    }
}