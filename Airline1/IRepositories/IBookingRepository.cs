using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IBookingRepository
    {
        Task<Booking> AddAsync(Booking booking);
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking?> GetByCodeAsync(string code);
        Task<IEnumerable<Booking>> GetByFlightIdAsync(int flightId);
        Task SaveChangesAsync();
        Task CancelAsync(Booking booking);
        void Update(Booking booking);
    }
}