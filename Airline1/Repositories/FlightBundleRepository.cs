using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    // Primary constructor syntax is clean and correct
    public class FlightBundleRepository(AppDbContext context) : IFlightBundleRepository
    {
        public async Task<IEnumerable<FlightBundle>> GetAllAsync()
        {
            return await context.FlightBundles.ToListAsync();
        }

        public async Task<FlightBundle?> GetByIdAsync(int id)
        {
            return await context.FlightBundles.FindAsync(id);
        }

        public async Task AddAsync(FlightBundle bundle)
        {
            await context.FlightBundles.AddAsync(bundle);
        }

        public Task UpdateAsync(FlightBundle bundle)
        {
            context.FlightBundles.Update(bundle);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await context.FlightBundles.FindAsync(id);
            if (existing != null)
            {
                context.FlightBundles.Remove(existing);
            }
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}