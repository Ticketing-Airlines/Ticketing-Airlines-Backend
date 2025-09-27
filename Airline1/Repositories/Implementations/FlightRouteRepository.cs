using System.Collections.Generic;
using System.Threading.Tasks;
using Airline1.Models;
using Airline1.Repositories.Interfaces;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Airline1.Repositories.Implementations
{
    public class FlightRouteRepository : IFlightRouteRepository
    {
        private readonly AppDbContext _db;
        public FlightRouteRepository(AppDbContext db) { _db = db; }

        public async Task<FlightRoute> AddAsync(FlightRoute route)
        {
            _db.FlightRoutes.Add(route);
            await _db.SaveChangesAsync();
            return route;
        }

        public async Task<List<FlightRoute>> GetAllAsync()
        {
            return await _db.FlightRoutes
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FlightRoute> GetByIdAsync(int id)
        {
            return await _db.FlightRoutes
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(FlightRoute route)
        {
            _db.FlightRoutes.Update(route);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(FlightRoute route)
        {
            _db.FlightRoutes.Remove(route);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.FlightRoutes.AnyAsync(r => r.Id == id);
        }

        public async Task<FlightRoute> GetByOriginDestinationAsync(int originId, int destinationId)
        {
            return await _db.FlightRoutes
                .Include(r => r.OriginAirport)
                .Include(r => r.DestinationAirport)
                .FirstOrDefaultAsync(r => r.OriginAirportId == originId && r.DestinationAirportId == destinationId);
        }
    }
}
