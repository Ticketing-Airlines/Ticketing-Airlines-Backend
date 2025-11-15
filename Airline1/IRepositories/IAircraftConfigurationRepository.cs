using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IAircraftConfigurationRepository
    {
        Task<IEnumerable<AircraftConfiguration>> GetAllAsync();
        Task<AircraftConfiguration?> GetByIdAsync(string id);
        Task AddAsync(AircraftConfiguration config);
        Task UpdateAsync(AircraftConfiguration config);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();
    }
}
