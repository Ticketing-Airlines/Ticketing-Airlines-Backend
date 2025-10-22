using Airline1.Models;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;
using Airline1.IRepositories;

namespace Airline1.Repositories
{
    public class AircraftRepository(AppDbContext db) : IAircraftRepository
    {
        public async Task<Aircraft> AddAsync(Aircraft aircraft)
        {
            db.Aircrafts.Add(aircraft);
            await db.SaveChangesAsync();
            return aircraft;
        }

        public async Task<List<Aircraft>> GetAllAsync()
        {
            return await db.Aircrafts
                .Include(a => a.BaseAirport)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Aircraft?> GetByIdAsync(int id)
        {
            return await db.Aircrafts
                .Include(a => a.BaseAirport)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Aircraft aircraft)
        {
            db.Aircrafts.Update(aircraft);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Aircraft aircraft)
        {
            db.Aircrafts.Remove(aircraft);
            await db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await db.Aircrafts.AnyAsync(a => a.Id == id);
        }

        public async Task<Aircraft?> GetByTailNumberAsync(string tailNumber)
        {
            if (string.IsNullOrWhiteSpace(tailNumber)) return null;
            var tn = tailNumber.Trim();
            return await db.Aircrafts
                .Include(a => a.BaseAirport)
                .FirstOrDefaultAsync(a => a.TailNumber == tn);
        }
    }
}
