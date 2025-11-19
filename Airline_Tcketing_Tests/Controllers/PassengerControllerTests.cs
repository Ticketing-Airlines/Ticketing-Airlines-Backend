using Airline_Ticketing.Controllers;
using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Airline_Tcketing_Tests.Controllers
{
    public class PassengerControllerTests
    {
        private readonly Mock<IPassengerService> _mockPassengerService;
        private readonly PassengerController _controller;

        public PassengerControllerTests()
        {
            _mockPassengerService = new Mock<IPassengerService>();
            _controller = new PassengerController(_mockPassengerService.Object);
        }

        #region GetAllPassengers Tests

        [Fact]
        public async Task GetAllPassengers_WhenPassengersExist_ReturnsOkWithPassengers()
        {
            // Arrange
            var passengers = new List<PassengerResponse>
            {
                new PassengerResponse { Id = 1, Name = "John Doe", DateOfBirth = new DateOnly(1990, 1, 15), Nationality = "USA" },
                new PassengerResponse { Id = 2, Name = "Jane Smith", DateOfBirth = new DateOnly(1985, 5, 20), Nationality = "UK" }
            };

            _mockPassengerService.Setup(s => s.GetAllPassengersAsync())
                .ReturnsAsync(passengers);

            // Act
            var result = await _controller.GetAllPassengers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPassengers = Assert.IsAssignableFrom<IEnumerable<PassengerResponse>>(okResult.Value);
            Assert.Equal(2, returnedPassengers.Count());
        }

        [Fact]
        public async Task GetAllPassengers_WhenNoPassengers_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockPassengerService.Setup(s => s.GetAllPassengersAsync())
                .ReturnsAsync(new List<PassengerResponse>());

            // Act
            var result = await _controller.GetAllPassengers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPassengers = Assert.IsAssignableFrom<IEnumerable<PassengerResponse>>(okResult.Value);
            Assert.Empty(returnedPassengers);
        }

        #endregion

        #region GetPassengerById Tests

        [Fact]
        public async Task GetPassengerById_WhenPassengerExists_ReturnsOkWithPassenger()
        {
            // Arrange
            var passenger = new PassengerResponse
            {
                Id = 1,
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "USA"
            };

            _mockPassengerService.Setup(s => s.GetPassengerByIdAsync(1))
                .ReturnsAsync(passenger);

            // Act
            var result = await _controller.GetPassengerById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPassenger = Assert.IsType<PassengerResponse>(okResult.Value);
            Assert.Equal(1, returnedPassenger.Id);
            Assert.Equal("John Doe", returnedPassenger.Name);
        }

        [Fact]
        public async Task GetPassengerById_WhenPassengerDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockPassengerService.Setup(s => s.GetPassengerByIdAsync(999))
                .ReturnsAsync((PassengerResponse)null);

            // Act
            var result = await _controller.GetPassengerById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        #endregion

        #region CreatePassenger Tests

        [Fact]
        public async Task CreatePassenger_WithValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new CreatePassengerRequest
            {
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "USA"
            };

            var createdPassenger = new PassengerResponse
            {
                Id = 1,
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "USA"
            };

            _mockPassengerService.Setup(s => s.CreatePassengerAsync(request))
                .ReturnsAsync(createdPassenger);

            // Act
            var result = await _controller.CreatePassenger(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedPassenger = Assert.IsType<PassengerResponse>(createdResult.Value);
            Assert.Equal(1, returnedPassenger.Id);
            Assert.Equal("John Doe", returnedPassenger.Name);
        }

        [Fact]
        public async Task CreatePassenger_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreatePassengerRequest();
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreatePassenger(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion

        #region UpdatePassenger Tests

        [Fact]
        public async Task UpdatePassenger_WithValidRequest_ReturnsOkWithUpdatedPassenger()
        {
            // Arrange
            var request = new UpdatePassengerRequest
            {
                Name = "John Smith",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "Canada"
            };

            var updatedPassenger = new PassengerResponse
            {
                Id = 1,
                Name = "John Smith",
                DateOfBirth = new DateOnly(1990, 1, 15),
                Nationality = "Canada"
            };

            _mockPassengerService.Setup(s => s.UpdatePassengerAsync(1, request))
                .ReturnsAsync(updatedPassenger);

            // Act
            var result = await _controller.UpdatePassenger(1, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPassenger = Assert.IsType<PassengerResponse>(okResult.Value);
            Assert.Equal("Canada", returnedPassenger.Nationality);
        }

        [Fact]
        public async Task UpdatePassenger_WhenPassengerDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var request = new UpdatePassengerRequest
            {
                Name = "John Doe",
                DateOfBirth = new DateOnly(1990, 1, 15)
            };

            _mockPassengerService.Setup(s => s.UpdatePassengerAsync(999, request))
                .ReturnsAsync((PassengerResponse)null);

            // Act
            var result = await _controller.UpdatePassenger(999, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdatePassenger_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdatePassengerRequest();
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdatePassenger(1, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        #endregion

        #region DeletePassenger Tests

        [Fact]
        public async Task DeletePassenger_WhenPassengerExists_ReturnsNoContent()
        {
            // Arrange
            _mockPassengerService.Setup(s => s.DeletePassengerAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePassenger(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePassenger_WhenPassengerDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockPassengerService.Setup(s => s.DeletePassengerAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePassenger(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion
    }
}
