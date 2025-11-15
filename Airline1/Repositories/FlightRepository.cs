using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class FlightRepository(AppDbContext db) : IFlightRepository
    {
        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            return await db.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Route!)
                    .ThenInclude(r => r.OriginAirport)
                .Include(f => f.Route!) 
                    .ThenInclude(r => r.DestinationAirport)
                .ToListAsync();
        }

        public async Task<Flight?> GetByIdAsync(int id)
        {
            return await db.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Route)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(Flight flight) => await db.Flights.AddAsync(flight);

        public void UpdateAsync(Flight flight) => db.Flights.Update(flight);

        public void DeleteAsync(Flight flight) => db.Flights.Remove(flight);

        public async Task SaveChangesAsync() => await db.SaveChangesAsync();
    }
}
