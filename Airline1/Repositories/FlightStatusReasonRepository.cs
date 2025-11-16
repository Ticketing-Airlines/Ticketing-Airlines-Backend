using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class FlightStatusReasonRepository(AppDbContext db) : IFlightStatusReasonRepository
    {
        public async Task<IEnumerable<FlightStatusReason>> GetAllAsync()
        {
            return await db.FlightStatusReasons
                .AsNoTracking()
                .OrderBy(r => r.Code)
                .ToListAsync();
        }

        public async Task<FlightStatusReason?> GetByIdAsync(int id)
        {
            return await db.FlightStatusReasons.FindAsync(id);
        }

        public async Task<FlightStatusReason?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            var key = code.Trim().ToUpperInvariant();
            return await db.FlightStatusReasons
                .FirstOrDefaultAsync(r => r.Code.Equals(key, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task AddAsync(FlightStatusReason entity)
        {
            await db.FlightStatusReasons.AddAsync(entity);
        }

        public void Update(FlightStatusReason entity)
        {
            db.FlightStatusReasons.Update(entity);
        }

        public void Remove(FlightStatusReason entity)
        {
            db.FlightStatusReasons.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
    }
}
