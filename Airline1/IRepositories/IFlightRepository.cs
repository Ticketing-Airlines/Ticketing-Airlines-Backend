using Airline1.Dtos.Requests;
using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetAllAsync();
        Task<Flight?> GetByIdAsync(int id);

        // Search for flights based on origin, destination, and departure date
        Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime departureDate);

        // Enhanced search with full includes for flight search API
        Task<IEnumerable<Flight>> SearchAsync(SearchFlightRequest request);

        Task<Flight?> GetByFlightNumberAndDateAsync(string flightNumber, DateTime date);

        Task AddAsync(Flight flight);
        void UpdateAsync(Flight flight);
        void DeleteAsync(Flight flight);
        Task SaveChangesAsync();
    }
}
