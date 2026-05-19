using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockService = new Mock<IAuthService>();
            _controller = new AuthController(_mockService.Object);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalid()
        {
            var request = new LoginRequest { Email = "x@x.com", Password = "bad" };
            _mockService.Setup(s => s.LoginAsync(request)).ReturnsAsync((AuthResponse?)null);
            var result = await _controller.Login(request);
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenValid()
        {
            var request = new LoginRequest { Email = "u@u.com", Password = "pwd" };
            var resp = new AuthResponse { UserId = "1", Email = "u@u.com", SessionToken = "t" };
            _mockService.Setup(s => s.LoginAsync(request)).ReturnsAsync(resp);
            var result = await _controller.Login(request);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resp, ok.Value);
        }

        [Fact]
        public async Task Logout_ReturnsOk_WhenSuccess()
        {
            _mockService.Setup(s => s.LogoutAsync("token")).ReturnsAsync(true);
            var result = await _controller.Logout("token");
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Logout_ReturnsBadRequest_WhenFail()
        {
            _mockService.Setup(s => s.LogoutAsync("bad")).ReturnsAsync(false);
            var result = await _controller.Logout("bad");
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
