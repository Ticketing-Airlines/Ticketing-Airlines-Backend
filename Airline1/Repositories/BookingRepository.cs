using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class BookingRepository(AppDbContext context) : IBookingRepository
    {
        public async Task<Booking> AddAsync(Booking booking)
        {
            await context.Bookings.AddAsync(booking);
            return booking;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await context.Bookings
                .Include(b => b.Passengers)
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking?> GetByCodeAsync(string code)
        {
            return await context.Bookings
                .Include(b => b.Passengers)
                .Include(b => b.Flight)
                .FirstOrDefaultAsync(b => b.BookingCode == code);
        }

        public async Task<IEnumerable<Booking>> GetByFlightIdAsync(int flightId)
        {
            return await context.Bookings
                .Where(b => b.FlightId == flightId)
                .Include(b => b.Passengers)
                .ToListAsync();
        }

        public void Update(Booking booking)
        {
            context.Bookings.Update(booking);
        }
        public async Task SaveChangesAsync() => await context.SaveChangesAsync();

        public async Task CancelAsync(Booking booking)
        {
            booking.Status = BookingStatus.Cancelled;
            context.Bookings.Update(booking);
            await context.SaveChangesAsync();
        }
    }
}
