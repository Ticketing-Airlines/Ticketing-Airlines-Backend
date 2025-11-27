using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IFlightBundleRepository
    {
        Task<IEnumerable<FlightBundle>> GetAllAsync();
        Task<FlightBundle?> GetByIdAsync(int id);
        Task AddAsync(FlightBundle bundle);
        Task UpdateAsync(FlightBundle bundle); 
        Task DeleteAsync(int id);            
        Task SaveChangesAsync();
    }
}