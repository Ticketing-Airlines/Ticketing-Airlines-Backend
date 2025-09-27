using System.Collections.Generic;
using System.Threading.Tasks;
using Airline1.Models;

namespace Airline1.Repositories.Interfaces
{
    public interface IAirportRepository
    {
        Task<Airport> AddAsync(Airport airport);
        Task<List<Airport>> GetAllAsync();
        Task<Airport> GetByIdAsync(int id);
        Task UpdateAsync(Airport airport);
        Task DeleteAsync(Airport airport);
        Task<bool> ExistsAsync(int id);
    }
}
