using System.Collections.Generic;
using System.Threading.Tasks;
using Airline1.Models;
using Airline1.Repositories.Interfaces;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Airline1.Repositories.Implementations
{
    public class AircraftRepository : IAircraftRepository
    {
        private readonly AppDbContext _db;
        public AircraftRepository(AppDbContext db) { _db = db; }

        public async Task<Aircraft> AddAsync(Aircraft aircraft)
        {
            _db.Aircrafts.Add(aircraft);
            await _db.SaveChangesAsync();
            return aircraft;
        }

        public async Task<List<Aircraft>> GetAllAsync()
        {
            return await _db.Aircrafts
                .Include(a => a.BaseAirport)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Aircraft?> GetByIdAsync(int id)
        {
            return await _db.Aircrafts
                .Include(a => a.BaseAirport)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Aircraft aircraft)
        {
            _db.Aircrafts.Update(aircraft);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Aircraft aircraft)
        {
            _db.Aircrafts.Remove(aircraft);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Aircrafts.AnyAsync(a => a.Id == id);
        }

        public async Task<Aircraft?> GetByTailNumberAsync(string tailNumber)
        {
            if (string.IsNullOrWhiteSpace(tailNumber)) return null;
            var tn = tailNumber.Trim();
            return await _db.Aircrafts
                .Include(a => a.BaseAirport)
                .FirstOrDefaultAsync(a => a.TailNumber == tn);
        }
    }
}
