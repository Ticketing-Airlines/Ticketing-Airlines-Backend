using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IAddOnPriceRepository
    {
        Task<AddOnPrice> AddAsync(AddOnPrice price);
        Task UpdateAsync(AddOnPrice price);
        Task DeleteAsync(int id);
        Task<AddOnPrice?> GetByIdAsync(int id);
        Task<AddOnPrice?> GetActivePriceByFlightAndAddOnIdAsync(int flightId, int addOnId);
        Task<IEnumerable<AddOnPrice>> GetOverlappingRulesAsync(int flightId, int addOnId, DateTime validFrom, DateTime? validTo, int excludeId = 0);
        Task SaveChangesAsync();
    }
}