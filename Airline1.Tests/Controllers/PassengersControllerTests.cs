using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    public class PassengersControllerTests
    {
        private readonly Mock<IPassengerService> _mockService;
        private readonly PassengersController _controller;

        public PassengersControllerTests()
        {
            _mockService = new Mock<IPassengerService>();
            _controller = new PassengersController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_List()
        {
            var list = new List<object> { new { Id = 1 } };
            _mockService.Setup(s => s.GetAllAsync());
            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var req = new CreatePassengerRequest();
            var created = new { Id = 2 };
            _mockService.Setup(s => s.CreateAsync(req));
            var result = await _controller.Create(req);
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
