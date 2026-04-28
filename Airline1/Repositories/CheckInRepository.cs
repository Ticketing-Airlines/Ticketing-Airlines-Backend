using Airline1.Data;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class CheckInRepository(AppDbContext db) : ICheckInRepository
    {
        private readonly AppDbContext _db = db;

        public async Task<Booking?> GetBookingForCheckInAsync(string pnr)
        {
            return await _db.Bookings
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.Flight)
                        .ThenInclude(f => f.Route)
                            .ThenInclude(r => r.OriginAirport)
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.Flight)
                        .ThenInclude(f => f.Route)
                            .ThenInclude(r => r.DestinationAirport)
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.Flight)
                        .ThenInclude(f => f.Aircraft)
                .Include(b => b.Passengers)
                    .ThenInclude(p => p.FlightSeat)
                        .ThenInclude(fs => fs!.Seat)
                .Include(b => b.Passengers)
                    .ThenInclude(p => p.AddOns)
                        .ThenInclude(ba => ba.AddOnPrice)
                            .ThenInclude(ap => ap!.AddOn)
                .Include(b => b.FlightBundle)
                .FirstOrDefaultAsync(b => b.Pnr == pnr);
        }

        public async Task<List<CheckIn>> GetCheckInsByBookingIdAsync(Guid bookingId)
        {
            return await _db.CheckIns
                .Where(c => c.BookingId == bookingId)
                .ToListAsync();
        }

        public async Task<CheckIn?> GetCheckInByPassengerIdAsync(Guid passengerId)
        {
            return await _db.CheckIns
                .FirstOrDefaultAsync(c => c.PassengerId == passengerId);
        }

        public async Task<FlightSeat?> GetFlightSeatByFlightAndSeatNumberAsync(int flightId, string seatNumber)
        {
            return await _db.FlightSeats
                .Include(fs => fs.Seat)
                .FirstOrDefaultAsync(fs => fs.FlightId == flightId && fs.Seat!.SeatNumber == seatNumber);
        }

        public async Task<List<FlightSeat>> GetAvailableSeatsForFlightAsync(int flightId, int limit)
        {
            return await _db.FlightSeats
                .Include(fs => fs.Seat)
                .Where(fs => fs.FlightId == flightId && fs.Status == "Available")
                .Take(limit)
                .ToListAsync();
        }

        public async Task AddCheckInAsync(CheckIn checkIn)
        {
            await _db.CheckIns.AddAsync(checkIn);
        }

        public async Task AddBoardingPassAsync(BoardingPass boardingPass)
        {
            await _db.BoardingPasses.AddAsync(boardingPass);
        }

        public Task UpdateFlightSeatAsync(FlightSeat flightSeat)
        {
            _db.FlightSeats.Update(flightSeat);
            return Task.CompletedTask;
        }

        public Task UpdateBookingAsync(Booking booking)
        {
            _db.Bookings.Update(booking);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}