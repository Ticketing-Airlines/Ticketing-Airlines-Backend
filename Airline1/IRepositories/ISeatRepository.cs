using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface ISeatRepository
    {
        Task<List<Seat>> GetByAircraftIdAsync(int aircraftId);
        Task DeleteRangeAsync(IEnumerable<Seat> seats);
        Task SaveChangesAsync();
        Task<Seat> AddAsync(Seat seat);
        Task<Seat?> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetByAircraftAsync(int aircraftId);
        Task<Seat> UpdateAsync(Seat seat);
        Task DeleteAsync(Seat seat);
    }
}
