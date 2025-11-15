using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IFlightPriceRepository
    {
        Task<IEnumerable<FlightPrice>> GetByFlightAsync(int flightId);
        Task<IEnumerable<FlightPrice>> GetByFlightAndCabinAsync(int flightId, string cabinClass);
        Task<FlightPrice?> GetByIdAsync(int id);
        Task<FlightPrice?> GetActivePromoAsync(int flightId, string cabinClass, DateTime when);
        Task<FlightPrice?> GetActiveStandardAsync(int flightId, string cabinClass, DateTime when);
        Task AddAsync(FlightPrice price);
        Task UpdateAsync(FlightPrice price);
        Task DeleteAsync(FlightPrice price);
        Task SaveChangesAsync();
    }
}
