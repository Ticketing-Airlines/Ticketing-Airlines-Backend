using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;


namespace Airline1.Repositories
{
    public class FlightAddOnRepository(AppDbContext db) : IFlightAddOnRepository
    {
        public async Task<IEnumerable<FlightAddOn>> GetAllAsync()
        {
            return await db.FlightAddOns
                .AsNoTracking()
                .OrderBy(a => a.Category)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<FlightAddOn>> GetByCategoryAsync(AddOnCategory category)
        {
            return await db.FlightAddOns
                .AsNoTracking()
                .Where(a => a.Category == category)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<FlightAddOn?> GetByIdAsync(int id)
        {
            return await db.FlightAddOns.FindAsync(id);
        }

        public async Task<FlightAddOn?> GetByCodeAsync(string code)
        {
            return await db.FlightAddOns
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Code == code);
        }

        public async Task AddAsync(FlightAddOn addOn)
        {
            await db.FlightAddOns.AddAsync(addOn);
        }

        public Task UpdateAsync(FlightAddOn addOn)
        {
            db.FlightAddOns.Update(addOn);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await db.FlightAddOns.FindAsync(id);
            if (existing != null)
                db.FlightAddOns.Remove(existing);
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
