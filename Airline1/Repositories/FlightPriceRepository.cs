using Airline1.Common;
using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline1.Repositories
{
    public class FlightPriceRepository(AppDbContext context) : IFlightPriceRepository
    {
        // --- Standard CRUD Implementations ---

        public async Task<FlightPrice?> GetByIdAsync(int id)
        {
            return await context.FlightPrices
                .Include(p => p.FlightBundle) // Include bundle details for DTO mapping
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<FlightPrice>> GetAllByFlightAsync(int flightId)
        {
            return await context.FlightPrices
                .Where(p => p.FlightId == flightId)
                .Include(p => p.FlightBundle)
                .OrderByDescending(p => p.EffectiveFrom)
                .ToListAsync();
        }

        public async Task AddAsync(FlightPrice price)
        {
            await context.FlightPrices.AddAsync(price);
        }

        public Task UpdateAsync(FlightPrice price)
        {
            context.FlightPrices.Update(price);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var price = await GetByIdAsync(id);
            if (price != null)
            {
                context.FlightPrices.Remove(price);
            }
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        // --- Temporal Lookup Implementations ---

        /// <summary>
        /// Retrieves the single active FlightPrice record for a specific combination and time 't'.
        /// </summary>
        public async Task<FlightPrice?> GetActivePriceAsync(int flightId, string cabinClass, int flightBundleId, string passengerType, DateTime t)
        {
            return await context.FlightPrices
                .Include(p => p.FlightBundle)
                .Where(p => p.FlightId == flightId &&
                            p.CabinClass == cabinClass &&
                            p.FlightBundleId == flightBundleId && // Match the specific bundle
                            p.PassengerType == passengerType &&
                            p.EffectiveFrom <= t &&               // Price is effective now or in the past
                            (p.EffectiveTo == null || p.EffectiveTo > t)) // Price hasn't expired yet
                .OrderByDescending(p => p.EffectiveFrom) // Fallback: Take the latest if temporal overlap occurs
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves the currently active FlightPrice record for every unique Flight/CabinClass/Bundle combination.
        /// </summary>
        public async Task<IEnumerable<FlightPrice>> GetActivePricesByFlightAsync(int flightId)
        {
            var now = DateTime.UtcNow;

            return await context.FlightPrices
                .Include(p => p.FlightBundle)
                .Where(p => p.FlightId == flightId &&
                            p.EffectiveFrom <= now &&
                            (p.EffectiveTo == null || p.EffectiveTo > now))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the price record scheduled to start next for a specific combination.
        /// </summary>
        public async Task<FlightPrice?> GetNextScheduledPriceAsync(int flightId, string cabinClass, int flightBundleId)
        {
            var now = DateTime.UtcNow;

            return await context.FlightPrices
                .Where(p => p.FlightId == flightId &&
                            p.CabinClass == cabinClass &&
                            p.FlightBundleId == flightBundleId &&
                            p.EffectiveFrom > now) // Only look for future prices
                .OrderBy(p => p.EffectiveFrom) // Find the soonest one
                .FirstOrDefaultAsync();
        }
    }
}