using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Dtos.Requests;
using Airline1.Models;

namespace Airline1.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockRepo;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockRepo = new Mock<IAuthRepository>();
            _service = new AuthService(_mockRepo.Object);
        }

        [Fact]
        public async Task LoginAsync_ReturnsNull_WhenInvalid()
        {
            var req = new LoginRequest { Email = "x@x.com" , Password = "wrongpassword"};
            _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            var res = await _service.LoginAsync(req);
            Assert.Null(res);
        }
    }
}
