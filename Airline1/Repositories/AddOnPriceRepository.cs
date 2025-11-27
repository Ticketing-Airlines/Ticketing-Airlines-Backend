using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class AddOnPriceRepository(AppDbContext db) : IAddOnPriceRepository
    {
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

        // Implementation for Rule Overlap Prevention
        public async Task<IEnumerable<AddOnPrice>> GetOverlappingRulesAsync(
            int flightId, int addOnId, DateTime validFrom, DateTime? validTo, int excludeId = 0)
        {
            // Logic to check if the new period overlaps with any existing period
            // Overlap occurs if:
            // 1. Existing rule starts before the new rule ends (Existing.ValidFrom <= New.ValidTo)
            // 2. AND Existing rule ends after the new rule starts (Existing.ValidTo >= New.ValidFrom)

            // Normalize ValidTo to a high date if NULL for comparison ease
            DateTime newValidTo = validTo ?? DateTime.MaxValue.Date;

            return await db.AddOnPrices
                .AsNoTracking()
                .Where(p => p.FlightId == flightId &&
                            p.AddOnId == addOnId &&
                            p.AddOnPriceId != excludeId && // Exclude the rule itself during Update
                            (p.ValidFrom.Date <= newValidTo) && // Overlap condition 1
                            (p.ValidTo == null ? DateTime.MaxValue.Date : p.ValidTo.Value.Date) >= validFrom.Date) // Overlap condition 2
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}