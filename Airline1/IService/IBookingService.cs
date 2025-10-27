using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IBookingService
    {
        Task<BookingResponse?> CreateBookingAsync(CreateBookingRequest request);
        Task<BookingResponse?> GetByIdAsync(int id);
        Task<BookingResponse?> GetByCodeAsync(string code);
        Task<BookingResponse?> UpdateBookingAsync(int id, UpdateBookingRequest request);

        Task<bool> CancelBookingAsync(int id);
        Task<IEnumerable<BookingResponse>> GetByFlightAsync(int flightId);
    }
}
