using System.Collections.Generic;
using System.Threading.Tasks;
using Airline1.Models;
using Airline1.Repositories.Interfaces;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories.Implementations
{
    public class AirportRepository : IAirportRepository
    {
        private readonly AppDbContext _db;
        public AirportRepository(AppDbContext db) { _db = db; }

        public async Task<Airport> AddAsync(Airport airport)
        {
            _db.Airports.Add(airport);
            await _db.SaveChangesAsync();
            return airport;
        }

        public async Task<List<Airport>> GetAllAsync()
        {
            return await _db.Airports.AsNoTracking().ToListAsync();
        }

        public async Task<Airport> GetByIdAsync(int id)
        {
            return await _db.Airports.FindAsync(id);
        }

        public async Task UpdateAsync(Airport airport)
        {
            _db.Airports.Update(airport);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Airport airport)
        {
            _db.Airports.Remove(airport);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Airports.AnyAsync(a => a.Id == id);
        }
    }
}
