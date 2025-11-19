using Airline_Ticketing.Model;
using System.Runtime.CompilerServices;

namespace Airline_Ticketing.IRepository
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Tickets>> GetAllAsync();
        Task<Tickets?> GetByIdAsync(int id);

        Task<Tickets> AddAsync(Tickets ticket);

        Task<Tickets> UpdateAsync(Tickets ticket);

        Task<bool> DeleteAsync(int id);
     
     }
}
