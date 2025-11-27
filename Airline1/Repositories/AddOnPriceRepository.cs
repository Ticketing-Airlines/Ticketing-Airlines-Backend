using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Added for DateTime

namespace Airline1.Repositories
{
    public class AddOnPriceRepository(AppDbContext db) : IAddOnPriceRepository
    {
        // ----------------------------------------------------------------------
        // EXISTING CRUD/QUERY METHODS (Retained)
        // ----------------------------------------------------------------------

        public async Task<AddOnPrice> AddAsync(AddOnPrice price)
        {
            await db.AddOnPrices.AddAsync(price);
            return price;
        }

        public Task UpdateAsync(AddOnPrice price)
        {
            db.AddOnPrices.Update(price);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await db.AddOnPrices.FindAsync(id);
            if (existing != null)
                db.AddOnPrices.Remove(existing);
        }

        public async Task<AddOnPrice?> GetByIdAsync(int id)
        {
            return await db.AddOnPrices
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.AddOnPriceId == id);
        }

        public async Task<AddOnPrice?> GetActivePriceByFlightAndAddOnIdAsync(int flightId, int addOnId)
        {
            DateTime today = DateTime.UtcNow.Date;

            return await db.AddOnPrices
                .AsNoTracking()
                .Where(p => p.FlightId == flightId &&
                            p.AddOnId == addOnId &&
                            p.ValidFrom.Date <= today &&
                            (p.ValidTo == null || p.ValidTo.Value.Date >= today))
                .OrderByDescending(p => p.ValidFrom)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AddOnPrice>> GetOverlappingRulesAsync(
            int flightId, int addOnId, DateTime validFrom, DateTime? validTo, int excludeId = 0)
        {
            DateTime newValidTo = validTo ?? DateTime.MaxValue.Date;

            return await db.AddOnPrices
                .AsNoTracking()
                .Where(p => p.FlightId == flightId &&
                            p.AddOnId == addOnId &&
                            p.AddOnPriceId != excludeId &&
                            (p.ValidFrom.Date <= newValidTo) &&
                            (p.ValidTo == null ? DateTime.MaxValue.Date : p.ValidTo.Value.Date) >= validFrom.Date)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }

        // ----------------------------------------------------------------------
        // ⭐ NEW REQUIRED METHODS IMPLEMENTED ⭐
        // ----------------------------------------------------------------------

        public async Task<IEnumerable<AddOnPrice>> GetAllByFlightAddOnAsync(int flightAddOnId)
        {
            // Note: Assuming the relationship is FlightAddOn.Id == AddOnPrice.AddOnId
            return await db.AddOnPrices
                .AsNoTracking()
                .Where(p => p.AddOnId == flightAddOnId)
                .OrderByDescending(p => p.ValidFrom)
                .ToListAsync();
        }

        public async Task<IEnumerable<AddOnPrice>> GetPricesByIdsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return [];
            }

            // Fetches all AddOnPrice entities whose IDs are in the provided list.
            return await db.AddOnPrices
                .AsNoTracking()
                .Where(p => ids.Contains(p.AddOnPriceId))
                .ToListAsync();
        }
    }
}