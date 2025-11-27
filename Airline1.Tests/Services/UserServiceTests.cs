using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Dtos.Requests;
using Airline1.Models;

namespace Airline1.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _service = new UserService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetUserById_ReturnsNull_WhenMissing()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);
            var res = await _service.GetUserByIdAsync(99);
            Assert.Null(res);
        }
    }
}
