using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IRepositories
{
    public interface IFlightStatusRepository
    {
        Task<FlightStatus?> GetByIdAsync(int id);
        Task<FlightStatus?> GetLatestByFlightIdAsync(int flightId);
        Task<IEnumerable<FlightStatus>> GetHistoryByFlightIdAsync(int flightId, int limit = 100);
        Task AddAsync(FlightStatus entity);
        Task SaveChangesAsync();
    }
}
