using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.IServices;
using Airline_Ticketing.Model;
using Microsoft.EntityFrameworkCore;

namespace Airline_Ticketing.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // Get all bookings
        public async Task<IEnumerable<BookingResponse>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();

            return bookings.Select(b => new BookingResponse
            {
                BookingID = b.BookingID,
                UserID = b.UserID,
                FlightID = b.FlightID,
                BookingDate = b.BookingDate,
                TotalAmount = b.TotalAmount,
                Status = b.Status
            });
        }

        // Get booking by ID
        public async Task<BookingResponse?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return null;
            }

            return new BookingResponse
            {
                BookingID = booking.BookingID,
                UserID = booking.UserID,
                FlightID = booking.FlightID,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status
            };
        }

        // Get all bookings for a specific user
        public async Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);

            return bookings.Select(b => new BookingResponse
            {
                BookingID = b.BookingID,
                UserID = b.UserID,
                FlightID = b.FlightID,
                BookingDate = b.BookingDate,
                TotalAmount = b.TotalAmount,
                Status = b.Status
            });
        }

        // Create a new booking
        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            // Verify that the user exists
            var userExists = await _bookingRepository.UserExistsAsync(request.UserID);
            if (!userExists)
            {
                throw new InvalidOperationException($"User with ID {request.UserID} does not exist.");
            }

            // Verify that the flight exists
            var flightExists = await _bookingRepository.FlightExistsAsync(request.FlightID);
            if (!flightExists)
            {
                throw new InvalidOperationException($"Flight with ID {request.FlightID} does not exist.");
            }

            // Create new booking
            var newBooking = new Booking
            {
                UserID = request.UserID,
                FlightID = request.FlightID,
                BookingDate = request.BookingDate,
                TotalAmount = request.TotalAmount,
                Status = request.Status
            };

            // Add to database and save
            var createdBooking = await _bookingRepository.AddAsync(newBooking);

            // Return the booking response
            return new BookingResponse
            {
                BookingID = createdBooking.BookingID,
                UserID = createdBooking.UserID,
                FlightID = createdBooking.FlightID,
                BookingDate = createdBooking.BookingDate,
                TotalAmount = createdBooking.TotalAmount,
                Status = createdBooking.Status
            };
        }

        // Update an existing booking
        public async Task<BookingResponse?> UpdateBookingAsync(int id, UpdateBookingRequest request)
        {
            // Find the existing booking
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return null;
            }

            // Update booking properties (only update if values are provided)
            if (request.BookingDate.HasValue)
            {
                booking.BookingDate = request.BookingDate.Value;
            }

            if (request.TotalAmount.HasValue)
            {
                booking.TotalAmount = request.TotalAmount.Value;
            }

            if (request.Status.HasValue)
            {
                booking.Status = request.Status.Value;
            }

            // Save changes
            var updatedBooking = await _bookingRepository.UpdateAsync(booking);

            // Return updated booking
            return new BookingResponse
            {
                BookingID = updatedBooking.BookingID,
                UserID = updatedBooking.UserID,
                FlightID = updatedBooking.FlightID,
                BookingDate = updatedBooking.BookingDate,
                TotalAmount = updatedBooking.TotalAmount,
                Status = updatedBooking.Status
            };
        }

        // Delete a booking
        public async Task<bool> DeleteBookingAsync(int id)
        {
            return await _bookingRepository.DeleteAsync(id);
        }
    }
}
