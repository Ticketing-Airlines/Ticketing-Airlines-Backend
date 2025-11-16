using System.Collections.Generic;
using System.Threading.Tasks;
using Airline1.Models;

namespace Airline1.IRepositories
{
    public interface IFlightStatusReasonRepository
    {
        Task<IEnumerable<FlightStatusReason>> GetAllAsync();
        Task<FlightStatusReason?> GetByIdAsync(int id);
        Task<FlightStatusReason?> GetByCodeAsync(string code);
        Task AddAsync(FlightStatusReason entity);
        void Update(FlightStatusReason entity);
        void Remove(FlightStatusReason entity);
        Task SaveChangesAsync();
    }
}
