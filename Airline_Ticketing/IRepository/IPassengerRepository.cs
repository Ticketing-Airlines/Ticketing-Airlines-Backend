using Airline_Ticketing.Model;

namespace Airline_Ticketing.IRepository
{
    public interface IPassengerRepository
    {
        Task<IEnumerable<Passengers>> GetAllAsync();
        Task<Passengers?> GetByIdAsync(int id);
        Task<Passengers> AddAsync(Passengers passenger);
        Task<Passengers> UpdateAsync(Passengers passenger);
        Task<bool> DeleteAsync(int id);
    }
}
