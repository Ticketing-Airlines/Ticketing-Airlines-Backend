using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.Model;
using Airline_Ticketing.Service;
using Moq;
using Xunit;

namespace Airline_Tcketing_Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockRepository;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _mockRepository = new Mock<IBookingRepository>();
            _service = new BookingService(_mockRepository.Object);
        }

        #region GetAllBookingsAsync Tests

        [Fact]
        public async Task GetAllBookingsAsync_WhenBookingsExist_ReturnsBookingResponses()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking
                {
                    BookingID = 1,
                    UserID = 1,
                    FlightID = 10,
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    TotalAmount = 500.00m,
                },
                new Booking
                {
                    BookingID = 2,
                    UserID = 2,
                    FlightID = 20,
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    TotalAmount = 750.00m,
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(bookings);

            // Act
            var result = await _service.GetAllBookingsAsync();

            // Assert
            var bookingList = result.ToList();
            Assert.Equal(2, bookingList.Count);
            Assert.Equal(1, bookingList[0].BookingID);
            Assert.Equal(500.00m, bookingList[0].TotalAmount);
            Assert.Equal(2, bookingList[1].BookingID);
            Assert.Equal(750.00m, bookingList[1].TotalAmount);
        }

        [Fact]
        public async Task GetAllBookingsAsync_WhenNoBookings_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _service.GetAllBookingsAsync();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetBookingByIdAsync Tests

        [Fact]
        public async Task GetBookingByIdAsync_WhenBookingExists_ReturnsBookingResponse()
        {
            // Arrange
            var booking = new Booking
            {
                BookingID = 1,
                UserID = 1,
                FlightID = 10,
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = 500.00m,
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.GetBookingByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.BookingID);
            Assert.Equal(1, result.UserID);
            Assert.Equal(10, result.FlightID);
            Assert.Equal(500.00m, result.TotalAmount);
        }

        [Fact]
        public async Task GetBookingByIdAsync_WhenBookingDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Booking)null);

            // Act
            var result = await _service.GetBookingByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetBookingsByUserIdAsync Tests

        [Fact]
        public async Task GetBookingsByUserIdAsync_WhenUserHasBookings_ReturnsBookingResponses()
        {
            // Arrange
            var userId = 1;
            var bookings = new List<Booking>
            {
                new Booking
                {
                    BookingID = 1,
                    UserID = userId,
                    FlightID = 10,
                    BookingDate =DateOnly.FromDateTime(DateTime.Now),
                    TotalAmount = 500.00m,
                },
                new Booking
                {
                    BookingID = 2,
                    UserID = userId,
                    FlightID = 20,
                    BookingDate = DateOnly.FromDateTime(DateTime.Now),
                    TotalAmount = 750.00m,
                }
            };

            _mockRepository.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(bookings);

            // Act
            var result = await _service.GetBookingsByUserIdAsync(userId);

            // Assert
            var bookingList = result.ToList();
            Assert.Equal(2, bookingList.Count);
            Assert.All(bookingList, b => Assert.Equal(userId, b.UserID));
        }

        [Fact]
        public async Task GetBookingsByUserIdAsync_WhenUserHasNoBookings_ReturnsEmptyList()
        {
            // Arrange
            var userId = 999;
            _mockRepository.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _service.GetBookingsByUserIdAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region CreateBookingAsync Tests

        [Fact]
        public async Task CreateBookingAsync_WithValidRequest_CreatesAndReturnsBooking()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                UserID = 1,
                FlightID = 10,
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = 500.00m,
            };

            var createdBooking = new Booking
            {
                BookingID = 1,
                UserID = request.UserID,
                FlightID = request.FlightID,
                BookingDate = request.BookingDate,
                TotalAmount = request.TotalAmount,
                Status = request.Status
            };

            _mockRepository.Setup(r => r.UserExistsAsync(request.UserID))
                .ReturnsAsync(true);

            _mockRepository.Setup(r => r.FlightExistsAsync(request.FlightID))
                .ReturnsAsync(true);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>()))
                .ReturnsAsync(createdBooking);

            // Act
            var result = await _service.CreateBookingAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.BookingID);
            Assert.Equal(1, result.UserID);
            Assert.Equal(10, result.FlightID);
            Assert.Equal(500.00m, result.TotalAmount);
        }

        [Fact]
        public async Task CreateBookingAsync_WhenUserDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                UserID = 999,
                FlightID = 10,
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = 500.00m,
            };

            _mockRepository.Setup(r => r.UserExistsAsync(request.UserID))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateBookingAsync(request)
            );
            Assert.Equal("User with ID 999 does not exist.", exception.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_WhenFlightDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                UserID = 1,
                FlightID = 999,
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = 500.00m,
            };

            _mockRepository.Setup(r => r.UserExistsAsync(request.UserID))
                .ReturnsAsync(true);

            _mockRepository.Setup(r => r.FlightExistsAsync(request.FlightID))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateBookingAsync(request)
            );
            Assert.Equal("Flight with ID 999 does not exist.", exception.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Never);
        }

        #endregion

        #region UpdateBookingAsync Tests

        [Fact]
        public async Task UpdateBookingAsync_WhenBookingExists_UpdatesAndReturnsBooking()
        {
            // Arrange
            var existingBooking = new Booking
            {
                BookingID = 1,
                UserID = 1,
                FlightID = 10,
                BookingDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1),
                TotalAmount = 500.00m,
            };

            var updateRequest = new UpdateBookingRequest
            {
                BookingDate = DateOnly.FromDateTime(DateTime.Now),
                TotalAmount = 600.00m,
                Status = Airline_Ticketing.Enums.BookingStatus.Confirmed
            };

            var updatedBooking = new Booking
            {
                BookingID = 1,
                UserID = 1,
                FlightID = 10,
                BookingDate = updateRequest.BookingDate.Value,
                TotalAmount = updateRequest.TotalAmount.Value,
                Status = updateRequest.Status.Value
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingBooking);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>()))
                .ReturnsAsync(updatedBooking);

            // Act
            var result = await _service.UpdateBookingAsync(1, updateRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.BookingID);
            Assert.Equal(600.00m, result.TotalAmount);
        }

        [Fact]
        public async Task UpdateBookingAsync_WhenBookingDoesNotExist_ReturnsNull()
        {
            // Arrange
            var updateRequest = new UpdateBookingRequest
            {
                TotalAmount = 600.00m
            };

            _mockRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Booking)null);

            // Act
            var result = await _service.UpdateBookingAsync(999, updateRequest);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task UpdateBookingAsync_UpdatesOnlyProvidedFields()
        {
            // Arrange
            var originalDate = DateTime.Now.AddDays(-1);
            var existingBooking = new Booking
            {
                BookingID = 1,
                UserID = 1,
                FlightID = 10,
                BookingDate = DateOnly.FromDateTime(originalDate),
                TotalAmount = 500.00m,
            };

            var updateRequest = new UpdateBookingRequest
            {
                TotalAmount = 600.00m
                // BookingDate and Status are not provided
            };

            Booking capturedBooking = null;

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingBooking);

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>()))
                .Callback<Booking>(b => capturedBooking = b)
                .ReturnsAsync((Booking b) => b);

            // Act
            await _service.UpdateBookingAsync(1, updateRequest);

            // Assert
            Assert.NotNull(capturedBooking);
            Assert.Equal(600.00m, capturedBooking.TotalAmount); // Updated
            Assert.Equal(DateOnly.FromDateTime(originalDate), capturedBooking.BookingDate);
        }

        #endregion

        #region DeleteBookingAsync Tests

        [Fact]
        public async Task DeleteBookingAsync_WhenBookingExists_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteBookingAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenBookingDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _service.DeleteBookingAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(999), Times.Once);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(999, false)]
        public async Task DeleteBookingAsync_WithVariousIds_ReturnsExpectedResult(int id, bool expectedResult)
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteAsync(id))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.DeleteBookingAsync(id);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion
    }
}
