using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetAllAsync();
        Task<Flight?> GetByIdAsync(int id);
        Task AddAsync(Flight flight);
        void UpdateAsync(Flight flight);
        void DeleteAsync(Flight flight);
        Task SaveChangesAsync();
    }
}
