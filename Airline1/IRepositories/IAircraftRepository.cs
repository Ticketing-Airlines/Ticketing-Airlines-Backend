using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IAircraftRepository
    {
        Task<Aircraft> AddAsync(Aircraft aircraft);
        Task<List<Aircraft>> GetAllAsync();
        Task<Aircraft?> GetByIdAsync(int id);
        Task UpdateAsync(Aircraft aircraft);
        Task DeleteAsync(Aircraft aircraft);
        Task<bool> ExistsAsync(int id);
        Task<Aircraft?> GetByTailNumberAsync(string tailNumber);
    }
}
