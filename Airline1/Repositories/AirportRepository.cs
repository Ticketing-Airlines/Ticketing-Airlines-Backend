using Airline1.Models;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;
using Airline1.IRepositories;

namespace Airline1.Repositories
{
    public class AirportRepository(AppDbContext db) : IAirportRepository
    {
        public async Task<Airport> AddAsync(Airport airport)
        {
            db.Airports.Add(airport);
            await db.SaveChangesAsync();
            return airport;
        }

        public async Task<List<Airport>> GetAllAsync()
        {
            return await db.Airports.AsNoTracking().ToListAsync();
        }

        public async Task<Airport?> GetByIdAsync(int id)
        {
            return await db.Airports.FindAsync(id);
        }

        public async Task UpdateAsync(Airport airport)
        {
            db.Airports.Update(airport);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Airport airport)
        {
            db.Airports.Remove(airport);
            await db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await db.Airports.AnyAsync(a => a.Id == id);
        }
    }
}
