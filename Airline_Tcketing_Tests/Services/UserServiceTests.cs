using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.Model;
using Airline_Ticketing.Service;
using Moq;
using Xunit;

namespace Airline_Tcketing_Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            _service = new UserService(_mockRepository.Object);
        }

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_WithValidRequest_CreatesAndReturnsUser()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "SecurePassword123",
                Phone = "1234567890"
            };

            _mockRepository.Setup(r => r.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Users>()))
                .ReturnsAsync((Users u) => new Users
                {
                    UserID = 1,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Password = u.Password,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt
                });

            // Act
            var result = await _service.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.Equal("1234567890", result.Phone);
        }

        [Fact]
        public async Task RegisterAsync_WithDuplicateEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com",
                Password = "SecurePassword123"
            };

            _mockRepository.Setup(r => r.EmailExistsAsync(request.Email))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RegisterAsync(request)
            );
            Assert.Equal("A user with this email already exists.", exception.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Users>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_HashesPassword()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "PlainTextPassword"
            };

            Users capturedUser = null;

            _mockRepository.Setup(r => r.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Users>()))
                .Callback<Users>(u => capturedUser = u)
                .ReturnsAsync((Users u) => new Users
                {
                    UserID = 1,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Password = u.Password,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt
                });

            // Act
            await _service.RegisterAsync(request);

            // Assert
            Assert.NotNull(capturedUser);
            Assert.NotEqual("PlainTextPassword", capturedUser.Password);
            Assert.NotEmpty(capturedUser.Password);
        }

        [Fact]
        public async Task RegisterAsync_SetsCreatedAtTimestamp()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password123"
            };

            Users capturedUser = null;
            var beforeCall = DateTime.UtcNow;

            _mockRepository.Setup(r => r.EmailExistsAsync(request.Email))
                .ReturnsAsync(false);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Users>()))
                .Callback<Users>(u => capturedUser = u)
                .ReturnsAsync((Users u) => new Users
                {
                    UserID = 1,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Password = u.Password,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt
                });

            // Act
            await _service.RegisterAsync(request);
            var afterCall = DateTime.UtcNow;

            // Assert
            Assert.NotNull(capturedUser);
            Assert.True(capturedUser.CreatedAt >= beforeCall);
            Assert.True(capturedUser.CreatedAt <= afterCall);
        }

        #endregion

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var password = "MyPassword123";
            var hashedPassword = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(password)
                )
            );

            var request = new LoginUserRequest
            {
                Email = "john@example.com",
                Password = password
            };

            var user = new Users
            {
                UserID = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = hashedPassword
            };

            _mockRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john@example.com", result.Email);
            Assert.NotNull(result.Token);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Login successful", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123"
            };

            _mockRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((Users)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.LoginAsync(request)
            );
            Assert.Equal("Invalid email or password.", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WithIncorrectPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var correctPassword = "CorrectPassword";
            var hashedPassword = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(correctPassword)
                )
            );

            var request = new LoginUserRequest
            {
                Email = "john@example.com",
                Password = "WrongPassword"
            };

            var user = new Users
            {
                UserID = 1,
                Email = "john@example.com",
                Password = hashedPassword
            };

            _mockRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.LoginAsync(request)
            );
            Assert.Equal("Invalid email or password.", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_GeneratesToken()
        {
            // Arrange
            var password = "Password123";
            var hashedPassword = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(password)
                )
            );

            var request = new LoginUserRequest
            {
                Email = "john@example.com",
                Password = password
            };

            var user = new Users
            {
                UserID = 1,
                Email = "john@example.com",
                Password = hashedPassword
            };

            _mockRepository.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            Assert.NotNull(result.Token);
            Assert.True(result.Token.Length > 0);
        }

        #endregion

        #region GetUserByIdAsync Tests

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUserResponse()
        {
            // Arrange
            var user = new Users
            {
                UserID = 1,
                FirstName = "John",
                MiddleName = "M",
                LastName = "Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("M", result.MiddleName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john@example.com", result.Email);
            Assert.Equal("1234567890", result.Phone);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Users)null);

            // Act
            var result = await _service.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUserByEmailAsync Tests

        [Fact]
        public async Task GetUserByEmailAsync_WhenUserExists_ReturnsUserResponse()
        {
            // Arrange
            var email = "john@example.com";
            var user = new Users
            {
                UserID = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                Phone = "1234567890",
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _mockRepository.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((Users)null);

            // Act
            var result = await _service.GetUserByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllUsersAsync Tests

        [Fact]
        public async Task GetAllUsersAsync_WhenUsersExist_ReturnsUserResponses()
        {
            // Arrange
            var users = new List<Users>
            {
                new Users
                {
                    UserID = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Phone = "1234567890",
                    CreatedAt = DateTime.UtcNow
                },
                new Users
                {
                    UserID = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane@example.com",
                    Phone = "0987654321",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            var userList = result.ToList();
            Assert.Equal(2, userList.Count);
            Assert.Equal("John", userList[0].FirstName);
            Assert.Equal("Jane", userList[1].FirstName);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenNoUsers_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Users>());

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_DoesNotReturnPasswords()
        {
            // Arrange
            var users = new List<Users>
            {
                new Users
                {
                    UserID = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Password = "HashedPassword",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            var userResponse = result.First();
            var properties = userResponse.GetType().GetProperties();
            Assert.DoesNotContain(properties, p => p.Name == "Password");
        }

        #endregion
    }
}
