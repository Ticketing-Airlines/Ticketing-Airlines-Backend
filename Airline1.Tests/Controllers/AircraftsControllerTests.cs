using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.Services.Interfaces;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.Tests.Controllers
{
    public class AircraftsControllerTests
    {
        private readonly Mock<IAircraftService> _mockService;
        private readonly AircraftsController _controller;

        public AircraftsControllerTests()
        {
            _mockService = new Mock<IAircraftService>();
            _controller = new AircraftsController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenValid()
        {
            var request = new CreateAircraftRequest {
                Model = "A320",
                Manufacturer = "AirCorp",
                TailNumber = "TN123",
                RegistrationNumber = "REG1"
            };
            var response = new AircraftResponse {
                Id = 1,
                Model = "A320",
                Manufacturer = "AirCorp",
                TailNumber = "TN123",
                RegistrationNumber = "REG1", 
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = null,
                BaseAirportName = "NAIA"
            };
            _mockService.Setup(x => x.CreateAsync(request)).ReturnsAsync(response);
            var result = await _controller.Create(request);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(response, createdResult.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfAircrafts()
        {
            var aircrafts = new List<AircraftResponse> {
                new AircraftResponse {
                    Id = 1,
                    Model = "A320",
                    Manufacturer = "AirCorp",
                    TailNumber = "TN123",
                    RegistrationNumber = "R1",
                    CreatedAt = System.DateTime.UtcNow,
                    UpdatedAt = null,
                    BaseAirportName = "NAIA"
                },
                new AircraftResponse {
                    Id = 2,
                    Model = "B737",
                    Manufacturer = "FlyInc",
                    TailNumber = "TN456",
                    RegistrationNumber = "R2",
                    CreatedAt = System.DateTime.UtcNow,
                    UpdatedAt = null,
                    BaseAirportName = "NAIA"
                }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(aircrafts);
            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<AircraftResponse>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenAircraftExists()
        {
            var aircraft = new AircraftResponse {
                Id = 1,
                Model = "A320",
                Manufacturer = "AirCorp",
                TailNumber = "TN123",
                RegistrationNumber = "R1",
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = null,
                BaseAirportName = "NAIA"
            };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(aircraft);
            var result = await _controller.GetById(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AircraftResponse>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenAircraftDoesNotExist()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((AircraftResponse?)null);
            var result = await _controller.GetById(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            var request = new UpdateAircraftRequest {
                Model = "A321",
                Manufacturer = "AirCorp",
                TailNumber = "TN123",
                RegistrationNumber = "R1"
            };
            var updatedResponse = new AircraftResponse {
                Id = 1,
                Model = "A321",
                Manufacturer = "AirCorp",
                TailNumber = "TN123",
                RegistrationNumber = "R1",
                CreatedAt = System.DateTime.UtcNow,
                UpdatedAt = System.DateTime.UtcNow,
                BaseAirportName = "NAIA"
            };
            _mockService.Setup(s => s.UpdateAsync(1, request)).ReturnsAsync(updatedResponse);
            var result = await _controller.Update(1, request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AircraftResponse>(okResult.Value);
            Assert.Equal("A321", returnValue.Model);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenAircraftDoesNotExist()
        {
            var request = new UpdateAircraftRequest {
                Model = "X",
                Manufacturer = "Unknown",
                TailNumber = "TN999",
                RegistrationNumber = "R999"
            };
            _mockService.Setup(s => s.UpdateAsync(99, request)).ReturnsAsync((AircraftResponse?)null);
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
        public async Task Delete_ReturnsNotFound_WhenAircraftDoesNotExist()
        {
            _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);
            var result = await _controller.Delete(99);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
