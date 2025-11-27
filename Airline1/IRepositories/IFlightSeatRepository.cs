using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IRepositories
{
    public interface IFlightSeatRepository
    {
        Task<FlightSeat?> GetByIdAsync(int id);
        Task<IEnumerable<FlightSeat>> GetByFlightAsync(int flightId);
        Task AddRangeAsync(IEnumerable<FlightSeat> flightSeats);
        Task AddAsync(FlightSeat seat);
        Task UpdateAsync(FlightSeat seat);
        Task DeleteRangeByFlightAsync(int flightId);
    }
}
