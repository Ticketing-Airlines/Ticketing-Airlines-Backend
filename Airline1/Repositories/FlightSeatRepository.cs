using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;


namespace Airline1.Repositories
{
    public class FlightSeatRepository(AppDbContext db) : IFlightSeatRepository
    {
        public async Task<FlightSeat?> GetByIdAsync(Guid id)
        {
            return await db.FlightSeats
                .Include(fs => fs.Seat)
                .AsNoTracking()
                .FirstOrDefaultAsync(fs => fs.FlightSeatId == id);
        }

        public async Task<IEnumerable<FlightSeat>> GetByFlightAsync(int flightId)
        {
            return await db.FlightSeats
                .Include(fs => fs.Seat)
                .Where(fs => fs.FlightId == flightId)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<FlightSeat> flightSeats)
        {
            await db.FlightSeats.AddRangeAsync(flightSeats);
            await db.SaveChangesAsync();
        }

        public async Task AddAsync(FlightSeat seat)
        {
            await db.FlightSeats.AddAsync(seat);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(FlightSeat seat)
        {
            db.FlightSeats.Update(seat);
            await db.SaveChangesAsync();
        }

        public async Task DeleteRangeByFlightAsync(int flightId)
        {
            var existing = await db.FlightSeats.Where(fs => fs.FlightId == flightId).ToListAsync();
            if (existing.Count != 0)
            {
                db.FlightSeats.RemoveRange(existing);
                await db.SaveChangesAsync();
            }
        }
    }
}
