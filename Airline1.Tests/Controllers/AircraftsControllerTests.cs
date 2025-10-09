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
            var request = new CreateAircraftRequest { Model = "A320", Manufacturer = "AirCorp" };
            var response = new AircraftResponse { Id = 1, Model = "A320", Manufacturer = "AirCorp", Registration = "REG1" };
            _mockService.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            var result = await _controller.Create(request);
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), created.ActionName);
            Assert.Equal(response, created.Value);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            var list = new List<AircraftResponse> {
                new AircraftResponse { Id = 1, Model = "A320", Manufacturer = "AirCorp", Registration = "R1" },
                new AircraftResponse { Id = 2, Model = "B737", Manufacturer = "FlyInc", Registration = "R2" }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(list);
            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<List<AircraftResponse>>(ok.Value);
            Assert.Equal(2, value.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var item = new AircraftResponse { Id = 1, Model = "A320", Manufacturer = "AirCorp", Registration = "R1" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(item);
            var result = await _controller.GetById(1);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var val = Assert.IsType<AircraftResponse>(ok.Value);
            Assert.Equal(1, val.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((AircraftResponse?)null);
            var result = await _controller.GetById(99);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUpdated()
        {
            var req = new UpdateAircraftRequest { Model = "A321" };
            var updated = new AircraftResponse { Id = 1, Model = "A321", Manufacturer = "AirCorp", Registration = "R1" };
            _mockService.Setup(s => s.UpdateAsync(1, req)).ReturnsAsync(updated);
            var result = await _controller.Update(1, req);
            var ok = Assert.IsType<OkObjectResult>(result);
            var val = Assert.IsType<AircraftResponse>(ok.Value);
            Assert.Equal("A321", val.Model);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenMissing()
        {
            var req = new UpdateAircraftRequest { Model = "X" };
            _mockService.Setup(s => s.UpdateAsync(99, req)).ReturnsAsync((AircraftResponse?)null);
            var result = await _controller.Update(99, req);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenMissing()
        {
            _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);
            var result = await _controller.Delete(99);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
