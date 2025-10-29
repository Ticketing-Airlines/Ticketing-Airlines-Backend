using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class AircraftConfigurationRepository(AppDbContext db) : IAircraftConfigurationRepository
    {
        public async Task<IEnumerable<AircraftConfiguration>> GetAllAsync() =>
            await db.AircraftConfigurations.Include(c => c.CabinDetails).ToListAsync();

        public async Task<AircraftConfiguration?> GetByIdAsync(string id) =>
            await db.AircraftConfigurations.Include(c => c.CabinDetails)
                .FirstOrDefaultAsync(c => c.ConfigurationID == id);

        public async Task AddAsync(AircraftConfiguration config) =>
            await db.AircraftConfigurations.AddAsync(config);

        public async Task UpdateAsync(AircraftConfiguration config)
        {
            db.AircraftConfigurations.Update(config);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) db.AircraftConfigurations.Remove(entity);
        }

        public async Task SaveChangesAsync() => await db.SaveChangesAsync();
    }
}
