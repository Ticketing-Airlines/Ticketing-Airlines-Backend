using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockService = new Mock<IUserService>();
            _controller = new UsersController(_mockService.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk()
        {
            var req = new RegisterRequest { Email = "a@a.com", Password = "p" };
            var resp = new { Id = 1, Email = "a@a.com" };
            _mockService.Setup(s => s.RegisterUserAsync(req)).ReturnsAsync(resp);
            var result = await _controller.Register(req);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resp, ok.Value);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkList()
        {
            var list = new List<object> { new { Id = 1 } };
            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(list);
            var result = await _controller.GetAllUsers();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }
    }
}
