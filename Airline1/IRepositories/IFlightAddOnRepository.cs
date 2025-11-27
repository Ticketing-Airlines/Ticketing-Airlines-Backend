using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IRepositories
{
    public interface IFlightAddOnRepository
    {
        Task<IEnumerable<FlightAddOn>> GetAllAsync();
        Task<IEnumerable<FlightAddOn>> GetByCategoryAsync(AddOnCategory category);
        Task<FlightAddOn?> GetByIdAsync(int id);
        Task<FlightAddOn?> GetByCodeAsync(string code);
        Task AddAsync(FlightAddOn addOn);
        Task UpdateAsync(FlightAddOn addOn);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
