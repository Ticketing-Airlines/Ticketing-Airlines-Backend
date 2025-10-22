using Airline1.Models;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;
using Airline1.IRepositories;

namespace Airline1.Repositories
{
    public class FlightRouteRepository(AppDbContext db) : IFlightRouteRepository
    {
        public async Task<FlightRoute> AddAsync(FlightRoute route)
        {
            db.FlightRoutes.Add(route);
            await db.SaveChangesAsync();
            return route;
        }

        public async Task<List<FlightRoute>> GetAllAsync()
        {
            return await db.FlightRoutes
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FlightRoute?> GetByIdAsync(int id)
        {
            return await db.FlightRoutes
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(FlightRoute route)
        {
            db.FlightRoutes.Update(route);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(FlightRoute route)
        {
            db.FlightRoutes.Remove(route);
            await db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await db.FlightRoutes.AnyAsync(r => r.Id == id);
        }

        public async Task<FlightRoute?> GetByOriginDestinationAsync(int originId, int destinationId)
        {
            return await db.FlightRoutes
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .FirstOrDefaultAsync(r => r.OriginAirportId == originId && r.DestinationAirportId == destinationId);
        }
    }
}
