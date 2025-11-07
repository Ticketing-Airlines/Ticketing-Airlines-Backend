
using Airline_Ticketing.Model;

namespace Airline_Ticketing.IRepository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
        Task<Booking> AddAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task<bool> DeleteAsync(int id);
        Task<bool> UserExistsAsync(int userId);
        Task<bool> FlightExistsAsync(int flightId);
    }
}
