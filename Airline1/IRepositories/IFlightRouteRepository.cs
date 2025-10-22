using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IFlightRouteRepository
    {
        Task<FlightRoute> AddAsync(FlightRoute route);
        Task<List<FlightRoute>> GetAllAsync();
        Task<FlightRoute?> GetByIdAsync(int id);
        Task UpdateAsync(FlightRoute route);
        Task DeleteAsync(FlightRoute route);
        Task<bool> ExistsAsync(int id);
        Task<FlightRoute?> GetByOriginDestinationAsync(int originId, int destinationId);
    }
}
