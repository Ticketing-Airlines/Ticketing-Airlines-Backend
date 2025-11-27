using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    // Renamed 'db' to '_context' for standard convention, but preserved dependency injection style
    public class SeatRepository(AppDbContext db) : ISeatRepository
    {
        private readonly AppDbContext _context = db; 

        public async Task<Seat> AddAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
            await _context.SaveChangesAsync(); // persist here to return Id
            return seat;
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Existing method to get seats by aircraft, keeping the original name
        public async Task<IEnumerable<Seat>> GetByAircraftAsync(int aircraftId)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.AircraftId == aircraftId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();
        }

        // 1. NEW METHOD: Get seats by Aircraft ID for bulk operations
        public async Task<List<Seat>> GetByAircraftIdAsync(int aircraftId)
        {
            // Note: Removed AsNoTracking() as these entities will be modified (deleted) later.
            return await _context.Seats
                .Where(s => s.AircraftId == aircraftId)
                .ToListAsync();
        }

        // 2. NEW METHOD: Marks a range of entities for deletion
        public async Task DeleteRangeAsync(IEnumerable<Seat> seats)
        {
            _context.Seats.RemoveRange(seats);
            // NOTE: Removed await Task.CompletedTask; as it adds no value.
            // The actual save happens in SaveChangesAsync().
            await Task.CompletedTask; 
        }

        // 3. NEW METHOD: Executes pending changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Seat> UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
            return seat;
        }

        public async Task DeleteAsync(Seat seat)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
    }
}