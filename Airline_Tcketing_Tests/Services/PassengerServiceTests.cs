using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.Model;
using Airline_Ticketing.Service;
using Moq;
using Xunit;

namespace Airline_Tcketing_Tests.Services
{
    public class PassengerServiceTests
    {
        private readonly Mock<IPassengerRepository> _mockRepository;
        private readonly PassengerService _service;

        public PassengerServiceTests()
        {
            _mockRepository = new Mock<IPassengerRepository>();
            _service = new PassengerService(_mockRepository.Object);
        }

        #region GetAllPassengersAsync Tests

        [Fact]
        public async Task GetAllPassengersAsync_WhenPassengersExist_ReturnsPassengerResponses()
        {
            // Arrange
            var passengers = new List<Passengers>
            {
                new Passengers
                {
                    PassengerID = 1,
                    Name = "John Doe",
                    DateOfBirth = new DateOnly(1990, 1, 15),
                    Nationality = "USA",
                    PassportNumber = "P123456"
                },
                new Passengers
                {
                    PassengerID = 2,
                    Name = "Jane Smith",
                    DateOfBirth = new DateOnly(1985, 5, 20),
                    Nationality = "UK",
                    PassportNumber = "P789012"
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(passengers);

            // Act
            var result = await _service.GetAllPassengersAsync();

            // Assert
            var passengerList = result.ToList();
            Assert.Equal(2, passengerList.Count);
            Assert.Equal(1, passengerList[0].Id);
            Assert.Equal("John Doe", passengerList[0].Name);
            Assert.Equal(new DateOnly(1990, 1, 15), passengerList[0].DateOfBirth);
            Assert.Equal("USA", passengerList[0].Nationality);
        }

        [Fact]
        public async Task GetAllPassengersAsync_WhenNoPassengers_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Passengers>());

            // Act
            var result = await _service.GetAllPassengersAsync();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetPassengerByIdAsync Tests

        [Fact]
        public async Task GetPassengerByIdAsync_WhenPassengerExists_ReturnsPassengerResponse()
        {
            // Arrange
            var passenger = new Passengers
            {
                PassengerID = 1,
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "USA",
                PassportNumber = "P123456"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(passenger);

            // Act
            var result = await _service.GetPassengerByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal(new DateOnly(1990, 1, 15), result.DateOfBirth);
            Assert.Equal("USA", result.Nationality);
        }

        [Fact]
        public async Task GetPassengerByIdAsync_WhenPassengerDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Passengers)null);

            // Act
            var result = await _service.GetPassengerByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task GetPassengerByIdAsync_WithVariousIds_CallsRepositoryCorrectly(int id)
        {
            // Arrange
            var passenger = new Passengers
            {
                PassengerID = id,
                Name = "Test Passenger",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Nationality = "Test"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(passenger);

            // Act
            var result = await _service.GetPassengerByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        #endregion

        #region CreatePassengerAsync Tests

        [Fact]
        public async Task CreatePassengerAsync_WithValidRequest_CreatesAndReturnsPassenger()
        {
            // Arrange
            var request = new CreatePassengerRequest
            {
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "USA",
                PassportNumber = "P123456"
            };

            var createdPassenger = new Passengers
            {
                PassengerID = 1,
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Nationality = request.Nationality,
                PassportNumber = request.PassportNumber
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Passengers>()))
                .ReturnsAsync(createdPassenger);

            // Act
            var result = await _service.CreatePassengerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal(new DateOnly(1990, 1, 15), result.DateOfBirth);
            Assert.Equal("USA", result.Nationality);
            _mockRepository.Verify(r => r.AddAsync(It.Is<Passengers>(p =>
                p.Name == request.Name &&
                p.DateOfBirth == request.DateOfBirth &&
                p.Nationality == request.Nationality &&
                p.PassportNumber == request.PassportNumber
            )), Times.Once);
        }

        [Fact]
        public async Task CreatePassengerAsync_MapsRequestToModelCorrectly()
        {
            // Arrange
            var request = new CreatePassengerRequest
            {
                Name = "Jane Smith",
                DateOfBirth = new DateOnly(1985, 5, 20),
                Nationality = "UK",
                PassportNumber = "P789012"
            };

            Passengers capturedPassenger = null;

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Passengers>()))
                .Callback<Passengers>(p => capturedPassenger = p)
                .ReturnsAsync((Passengers p) => new Passengers
                {
                    PassengerID = 2,
                    Name = p.Name,
                    DateOfBirth = p.DateOfBirth,
                    Nationality = p.Nationality,
                    PassportNumber = p.PassportNumber
                });

            // Act
            await _service.CreatePassengerAsync(request);

            // Assert
            Assert.NotNull(capturedPassenger);
            Assert.Equal(request.Name, capturedPassenger.Name);
            Assert.Equal(request.DateOfBirth, capturedPassenger.DateOfBirth);
            Assert.Equal(request.Nationality, capturedPassenger.Nationality);
            Assert.Equal(request.PassportNumber, capturedPassenger.PassportNumber);
        }

        #endregion

        #region UpdatePassengerAsync Tests

        [Fact]
        public async Task UpdatePassengerAsync_WhenPassengerExists_UpdatesAndReturnsPassenger()
        {
            // Arrange
            var existingPassenger = new Passengers
            {
                PassengerID = 1,
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "USA",
                PassportNumber = "P123456"
            };

            var updateRequest = new UpdatePassengerRequest
            {
                Name = "John Smith",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "Canada",
                PassportNumber = "P999999"
            };

            var updatedPassenger = new Passengers
            {
                PassengerID = 1,
                Name = updateRequest.Name,
                DateOfBirth = updateRequest.DateOfBirth,
                Nationality = updateRequest.Nationality,
                PassportNumber = updateRequest.PassportNumber
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingPassenger);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Passengers>()))
                .ReturnsAsync(updatedPassenger);

            // Act
            var result = await _service.UpdatePassengerAsync(1, updateRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("John Smith", result.Name);
            Assert.Equal("Canada", result.Nationality);
            Assert.Equal(new DateOnly(1990, 1, 15), result.DateOfBirth);
        }

        [Fact]
        public async Task UpdatePassengerAsync_WhenPassengerDoesNotExist_ReturnsNull()
        {
            // Arrange
            var updateRequest = new UpdatePassengerRequest
            {
                Name = "John Smith",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "Canada"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Passengers)null);

            // Act
            var result = await _service.UpdatePassengerAsync(999, updateRequest);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Passengers>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePassengerAsync_UpdatesAllProperties()
        {
            // Arrange
            var existingPassenger = new Passengers
            {
                PassengerID = 1,
                Name = "Old Name",
                DateOfBirth = new DateOnly(1990, 1, 1),
                Nationality = "Old Country",
                PassportNumber = "OLD123"
            };

            var updateRequest = new UpdatePassengerRequest
            {
                Name = "New Name",
                DateOfBirth = new DateOnly(1995, 12, 31),
                Nationality = "New Country",
                PassportNumber = "NEW999"
            };

            Passengers capturedPassenger = null;

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingPassenger);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Passengers>()))
                .Callback<Passengers>(p => capturedPassenger = p)
                .ReturnsAsync((Passengers p) => p);

            // Act
            await _service.UpdatePassengerAsync(1, updateRequest);

            // Assert
            Assert.NotNull(capturedPassenger);
            Assert.Equal("New Name", capturedPassenger.Name);
            Assert.Equal(new DateOnly(1995, 12, 31), capturedPassenger.DateOfBirth);
            Assert.Equal("New Country", capturedPassenger.Nationality);
            Assert.Equal("NEW999", capturedPassenger.PassportNumber);
        }

        #endregion

        #region DeletePassengerAsync Tests

        [Fact]
        public async Task DeletePassengerAsync_WhenPassengerExists_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeletePassengerAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeletePassengerAsync_WhenPassengerDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeletePassengerAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(999), Times.Once);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(999, false)]
        public async Task DeletePassengerAsync_WithVariousIds_ReturnsExpectedResult(int id, bool expectedResult)
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(id))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.DeletePassengerAsync(id);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion
    }
}
