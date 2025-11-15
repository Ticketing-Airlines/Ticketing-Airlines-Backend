using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;

namespace Airline_Ticketing.IServices
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingResponse>> GetAllBookingsAsync();
        Task<BookingResponse?> GetBookingByIdAsync(int id);
        Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(int userId);
        Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request);
        Task<BookingResponse?> UpdateBookingAsync(int id, UpdateBookingRequest request);
        Task<bool> DeleteBookingAsync(int id);
    }
}