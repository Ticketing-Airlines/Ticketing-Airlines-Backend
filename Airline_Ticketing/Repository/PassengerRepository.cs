
using Airline_Ticketing.Data;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.Model;
using Microsoft.EntityFrameworkCore;

namespace Airline_Ticketing.Repository
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly ApplicationDbContext _context;

        public PassengerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Passengers>> GetAllAsync()
        {
            return await _context.Passengers.ToListAsync();
        }

        public async Task<Passengers?> GetByIdAsync(int id)
        {
            return await _context.Passengers
                .FirstOrDefaultAsync(p => p.PassengerID == id);
        }

        public async Task<Passengers> AddAsync(Passengers passenger)
        {
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();
            return passenger;
        }

        public async Task<Passengers> UpdateAsync(Passengers passenger)
        {
            _context.Passengers.Update(passenger);
            await _context.SaveChangesAsync();
            return passenger;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var passenger = await _context.Passengers
                .FirstOrDefaultAsync(p => p.PassengerID == id);

            if (passenger == null)
            {
                return false;
            }

            _context.Passengers.Remove(passenger);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
