using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
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
            var req = new RegisterUserRequest {
                FirstName = "A", LastName = "B", PhoneNumber = "1234567890",
                Email = "a@a.com", Password = "p" };
            var resp = new RegisterResponse { Id = "1", Message = "Registration successful" };
            _mockService.Setup(s => s.RegisterUserAsync(req)).ReturnsAsync(resp);
            var result = await _controller.Register(req);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resp, ok.Value);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkList()
        {
            var list = new List<UserResponse> { new UserResponse { Id = "1", Email = "a@a.com", PhoneNumber = "123", Role = "User" } };
            _mockService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(list);
            var result = await _controller.GetAllUsers();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(list, ok.Value);
        }
    }
}
