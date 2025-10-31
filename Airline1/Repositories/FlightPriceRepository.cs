using Airline1.Data;
using Airline1.Models;
using Airline1.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class FlightPriceRepository(AppDbContext db) : IFlightPriceRepository
    {
        public async Task<IEnumerable<FlightPrice>> GetByFlightAsync(int flightId)
        {
            return await db.FlightPrices
                .Include(p => p.Flight)
                .Where(p => p.FlightId == flightId)
                .OrderByDescending(p => p.EffectiveFrom)
                .ToListAsync();
        }

        public async Task<IEnumerable<FlightPrice>> GetByFlightAndCabinAsync(int flightId, string cabinClass)
        {
            return await db.FlightPrices
                .Include(p => p.Flight)
                .Where(p => p.FlightId == flightId && p.CabinClass == cabinClass)
                .OrderByDescending(p => p.EffectiveFrom)
                .ToListAsync();
        }

        public async Task<FlightPrice?> GetByIdAsync(int id)
            => await db.FlightPrices.Include(p => p.Flight).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<FlightPrice?> GetActivePromoAsync(int flightId, string cabinClass, DateTime when)
        {
            return await db.FlightPrices
                .Where(p => p.FlightId == flightId
                            && p.CabinClass == cabinClass
                            && p.Type == FlightPriceType.Promo
                            && p.EffectiveFrom <= when
                            && (p.EffectiveTo == null || p.EffectiveTo >= when))
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefaultAsync();
        }

        public async Task<FlightPrice?> GetActiveStandardAsync(int flightId, string cabinClass, DateTime when)
        {
            return await db.FlightPrices
                .Where(p => p.FlightId == flightId
                            && p.CabinClass == cabinClass
                            && p.Type == FlightPriceType.Standard
                            && p.EffectiveFrom <= when
                            && (p.EffectiveTo == null || p.EffectiveTo >= when))
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(FlightPrice price) => await db.FlightPrices.AddAsync(price);
        public async Task UpdateAsync(FlightPrice price) => db.FlightPrices.Update(price);
        public async Task DeleteAsync(FlightPrice price) => db.FlightPrices.Remove(price);
        public async Task SaveChangesAsync() => await db.SaveChangesAsync();
    }
}
