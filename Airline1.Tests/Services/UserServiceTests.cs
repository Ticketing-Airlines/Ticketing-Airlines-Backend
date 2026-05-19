using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
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
            _mockRepo.Setup(r => r.GetByIdAsync("99")).ReturnsAsync((User?)null);
            var res = await _service.GetUserByIdAsync("99");
            Assert.Null(res);
        }

        [Fact]
        public async Task GetUserById_ReturnsUser_WhenFound()
        {
            var user = new User
            {
                Id = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                Email = "user@test.com",
                PhoneNumber = "1234567890",
                Role = "User"
            };
            _mockRepo.Setup(r => r.GetByIdAsync("user@test.com")).ReturnsAsync(user);
            var res = await _service.GetUserByIdAsync("user@test.com");
            Assert.NotNull(res);
            Assert.Equal("John Doe", res.FullName);
            Assert.Equal("user@test.com", res.Email);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsId_WhenSuccessful()
        {
            var req = new RegisterUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                Password = "password123",
                PhoneNumber = "1234567890"
            };
            var createdUser = new User { Id = "john@test.com", Email = "john@test.com" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

            var res = await _service.RegisterUserAsync(req);

            Assert.NotNull(res);
            Assert.Equal("john@test.com", res.Id);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsNull_WhenUserNotFound()
        {
            var req = new LoginUserRequest { Email = "notfound@test.com", Password = "password" };
            _mockRepo.Setup(r => r.GetByEmailAsync("notfound@test.com")).ReturnsAsync((User?)null);

            var res = await _service.LoginUserAsync(req);

            Assert.Null(res);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsNull_WhenPasswordInvalid()
        {
            var user = new User
            {
                Id = "user@test.com",
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };
            var req = new LoginUserRequest { Email = "user@test.com", Password = "wrongpassword" };
            _mockRepo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

            var res = await _service.LoginUserAsync(req);

            Assert.Null(res);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsResponse_WhenCredentialsValid()
        {
            var password = "correctpassword";
            var user = new User
            {
                Id = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                Email = "user@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User"
            };
            var req = new LoginUserRequest { Email = "user@test.com", Password = password };
            _mockRepo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

            var res = await _service.LoginUserAsync(req);

            Assert.NotNull(res);
            Assert.Equal("user@test.com", res.UserId);
            Assert.Equal("John Doe", res.FullName);
            Assert.Equal("User", res.Role);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsEmptyList_WhenNoUsers()
        {
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            var res = await _service.GetAllUsersAsync();

            Assert.NotNull(res);
            Assert.Empty(res);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsers_WhenUsersExist()
        {
            var users = new List<User>
            {
                new User { Id = "user1@test.com", FirstName = "John", LastName = "Doe", Email = "user1@test.com" },
                new User { Id = "user2@test.com", FirstName = "Jane", LastName = "Smith", Email = "user2@test.com" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var res = await _service.GetAllUsersAsync();

            Assert.NotNull(res);
            Assert.Equal(2, res.Count());
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsNull_WhenUserNotFound()
        {
            var req = new UpdateUserRequest { FirstName = "Updated" };
            _mockRepo.Setup(r => r.GetByIdAsync("notfound")).ReturnsAsync((User?)null);

            var res = await _service.UpdateUserAsync("notfound", req);

            Assert.Null(res);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsUpdatedUser_WhenSuccessful()
        {
            var user = new User
            {
                Id = "user@test.com",
                FirstName = "John",
                LastName = "Doe",
                Email = "user@test.com",
                PhoneNumber = "1234567890",
                Role = "User"
            };
            var req = new UpdateUserRequest { FirstName = "Updated" };
            var updatedUser = new User
            {
                Id = "user@test.com",
                FirstName = "Updated",
                LastName = "Doe",
                Email = "user@test.com",
                PhoneNumber = "1234567890",
                Role = "User"
            };
            _mockRepo.Setup(r => r.GetByIdAsync("user@test.com")).ReturnsAsync(user);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(updatedUser);

            var res = await _service.UpdateUserAsync("user@test.com", req);

            Assert.NotNull(res);
            Assert.Equal("Updated", res.FullName.Split(' ')[0]);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsFalse_WhenUserNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync("notfound")).ReturnsAsync((User?)null);

            var res = await _service.DeleteUserAsync("notfound");

            Assert.False(res);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsTrue_WhenSuccessful()
        {
            var user = new User { Id = "user@test.com", Email = "user@test.com" };
            _mockRepo.Setup(r => r.GetByIdAsync("user@test.com")).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteAsync(user)).Returns(Task.CompletedTask);

            var res = await _service.DeleteUserAsync("user@test.com");

            Assert.True(res);
        }
    }
}