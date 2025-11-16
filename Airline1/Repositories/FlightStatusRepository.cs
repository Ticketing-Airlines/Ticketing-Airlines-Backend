using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class FlightStatusRepository(AppDbContext db) : IFlightStatusRepository
    {
        public async Task AddAsync(FlightStatus entity)
        {
            await db.FlightStatuses.AddAsync(entity);
        }

        public async Task<FlightStatus?> GetByIdAsync(int id)
        {
            return await db.FlightStatuses
                .Include(s => s.Reason)
                .Include(s => s.Flight)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<FlightStatus>> GetHistoryByFlightIdAsync(int flightId, int limit = 100)
        {
            return await db.FlightStatuses
                .Where(s => s.FlightId == flightId)
                .OrderByDescending(s => s.EffectiveAt)
                .Take(limit)
                .Include(s => s.Reason)
                .ToListAsync();
        }

        public async Task<FlightStatus?> GetLatestByFlightIdAsync(int flightId)
        {
            return await db.FlightStatuses
                .Where(s => s.FlightId == flightId)
                .OrderByDescending(s => s.EffectiveAt)
                .Include(s => s.Reason)
                .Include(s => s.Flight)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
