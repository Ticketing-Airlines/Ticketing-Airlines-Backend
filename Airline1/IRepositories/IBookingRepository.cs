using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IRepositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking?> GetByPnrAsync(string pnr);
        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
        Task<Booking> AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task SaveChangesAsync();
        // Method to get a simple unique PNR
        string GenerateUniquePnr();
    }
}