using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IPassengerRepository
    {
        Task<IEnumerable<Passenger>> GetAllAsync();
        Task<Passenger?> GetByIdAsync(int id);
        Task<Passenger> AddAsync(Passenger passenger);
        Task<int> SaveChangesAsync();
        Task<Passenger?> UpdateAsync(Passenger passenger);
        Task<bool> DeleteAsync(int id);
    }
}
