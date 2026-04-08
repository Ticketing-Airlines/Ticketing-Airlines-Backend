using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline1.Repositories
{
    public class BookingRepository(AppDbContext db) : IBookingRepository
    {
        private readonly AppDbContext _db = db;

        // Note: Includes all navigation properties required for a full response
        private IQueryable<Booking> GetBookingQuery()
        {
            return _db.Bookings
                .Include(b => b.FlightBundle)
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.Flight)
                        .ThenInclude(f => f.Aircraft)
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.Flight)
                        .ThenInclude(f => f.Route)
                            .ThenInclude(r => r.OriginAirport)
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.Flight)
                        .ThenInclude(f => f.Route)
                            .ThenInclude(r => r.DestinationAirport)
                .Include(b => b.Passengers)
                    .ThenInclude(p => p.FlightSeat)
                        .ThenInclude(fs => fs!.Seat)
                .Include(b => b.Passengers)
                    .ThenInclude(p => p.AddOns)
                        .ThenInclude(ba => ba.AddOnPrice)
                            .ThenInclude(ap => ap!.AddOn);
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await GetBookingQuery().FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<Booking?> GetByPnrAsync(string pnr)
        {
            return await GetBookingQuery().FirstOrDefaultAsync(b => b.Pnr == pnr);
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
        {
            return await GetBookingQuery().Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            await _db.Bookings.AddAsync(booking);
            return booking;
        }

        public Task UpdateAsync(Booking booking)
        {
            _db.Bookings.Update(booking);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        public string GenerateUniquePnr()
        {
            const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            var random = new System.Random();
            string pnr;

            // Loop until a unique 6-character PNR is generated
            do
            {
                pnr = new string([.. Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)])]);
            } while (_db.Bookings.Any(b => b.Pnr == pnr));

            return pnr;
        }
    }
}