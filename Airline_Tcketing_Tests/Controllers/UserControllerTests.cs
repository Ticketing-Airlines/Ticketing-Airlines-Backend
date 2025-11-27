using Airline_Ticketing.Controllers;
using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Airline_Tcketing_Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        #region Register Tests

        [Fact]
        public async Task Register_WithValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Email = "test@example.com",
                Password = "SecurePassword123",
                FirstName = "John",
                LastName = "Doe"
            };

            var userResponse = new UserResponse
            {
                UserId = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserService.Setup(s => s.RegisterAsync(request))
                .ReturnsAsync(userResponse);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedUser = Assert.IsType<UserResponse>(createdResult.Value);
            Assert.Equal(1, returnedUser.UserId);
            Assert.Equal("test@example.com", returnedUser.Email);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Email = "existing@example.com",
                Password = "SecurePassword123"
            };

            _mockUserService.Setup(s => s.RegisterAsync(request))
                .ThrowsAsync(new InvalidOperationException("Email already exists"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterUserRequest();
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "test@example.com",
                Password = "SecurePassword123"
            };

            var loginResponse = new LoginResponse
            {
                Token = "jwt-token-here",
                UserId = 1,
                Email = "test@example.com"
            };

            _mockUserService.Setup(s => s.LoginAsync(request))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResponse = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.Equal("jwt-token-here", returnedResponse.Token);
            Assert.Equal(1, returnedResponse.UserId);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _mockUserService.Setup(s => s.LoginAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginUserRequest();
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion

        #region GetAllUsers Tests

        [Fact]
        public async Task GetAllUsers_WhenUsersExist_ReturnsOkWithUsers()
        {
            // Arrange
            var users = new List<UserResponse>
            {
                new UserResponse { UserId = 1, Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
                new UserResponse { UserId = 2, Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" }
            };

            _mockUserService.Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());
        }

        [Fact]
        public async Task GetAllUsers_WhenNoUsers_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetAllUsersAsync())
                .ReturnsAsync(new List<UserResponse>());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(okResult.Value);
            Assert.Empty(returnedUsers);
        }

        #endregion

        #region GetUserById Tests

        [Fact]
        public async Task GetUserById_WhenUserExists_ReturnsOkWithUser()
        {
            // Arrange
            var user = new UserResponse
            {
                UserId = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(1, returnedUser.UserId);
            Assert.Equal("test@example.com", returnedUser.Email);
        }

        [Fact]
        public async Task GetUserById_WhenUserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByIdAsync(999))
                .ReturnsAsync((UserResponse)null);

            // Act
            var result = await _controller.GetUserById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        #endregion

        #region GetUserByEmail Tests

        [Fact]
        public async Task GetUserByEmail_WhenUserExists_ReturnsOkWithUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new UserResponse
            {
                UserId = 1,
                Email = email,
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserService.Setup(s => s.GetUserByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(email, returnedUser.Email);
        }

        [Fact]
        public async Task GetUserByEmail_WhenUserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockUserService.Setup(s => s.GetUserByEmailAsync(email))
                .ReturnsAsync((UserResponse)null);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("admin@test.com")]
        [InlineData("user@domain.org")]
        public async Task GetUserByEmail_WithVariousEmails_CallsServiceCorrectly(string email)
        {
            // Arrange
            var user = new UserResponse
            {
                UserId = 1,
                Email = email
            };

            _mockUserService.Setup(s => s.GetUserByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(email, returnedUser.Email);
            _mockUserService.Verify(s => s.GetUserByEmailAsync(email), Times.Once);
        }

        #endregion
    }
}
