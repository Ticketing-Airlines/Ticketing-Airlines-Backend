using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class PassengerRepository(AppDbContext context) : IPassengerRepository
    {
        public async Task<IEnumerable<Passenger>> GetAllAsync()
        {
            return await context.Passengers.Include(p => p.User).ToListAsync();
        }

        public async Task<Passenger?> GetByIdAsync(int id)
        {
            return await context.Passengers.Include(p => p.User)
                                            .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Passenger> AddAsync(Passenger passenger)
        {
            context.Passengers.Add(passenger);
            await context.SaveChangesAsync();
            return passenger;
        }

        public async Task<Passenger?> UpdateAsync(Passenger passenger)
        {
            context.Passengers.Update(passenger);
            await context.SaveChangesAsync();
            return passenger;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var passenger = await context.Passengers.FindAsync(id);
            if (passenger == null) return false;

            context.Passengers.Remove(passenger);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
