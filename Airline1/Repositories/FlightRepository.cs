using Airline1.Data;
using Airline1.Dtos.Requests;
using Airline1.IRepositories;
using Airline1.Models;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Repositories
{
    public class FlightRepository(AppDbContext db) : IFlightRepository
    {
        public async Task<IEnumerable<Flight>> GetAllAsync()
        {
            return await db.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Route!)
                    .ThenInclude(r => r.OriginAirport)
                .Include(f => f.Route!)
                    .ThenInclude(r => r.DestinationAirport)
                .ToListAsync();
        }

        public async Task<Flight?> GetByIdAsync(int id)
        {
            return await db.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Route)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        // Implement the SearchAsync method to find flights
        public async Task<IEnumerable<Flight>> SearchAsync(string origin, string destination, DateTime departureDate)
        {
            // We search for flights where the route's origin/destination matches the input (IATA code or City)
            // and the departure date matches (ignoring the time part for the match)
            return await db.Flights
                .Include(f => f.Aircraft)
                .Include(f => f.Route!)
                    .ThenInclude(r => r.OriginAirport)
                .Include(f => f.Route!)
                    .ThenInclude(r => r.DestinationAirport)
                .Where(f =>
                    (f.Route!.OriginAirport.IataCode == origin || f.Route.OriginAirport.City == origin) &&
                    (f.Route!.DestinationAirport.IataCode == destination || f.Route.DestinationAirport.City == destination) &&
                    f.DepartureTime.Date == departureDate.Date)
                .ToListAsync();
        }

        // Enhanced search with full includes for flight search API
        public async Task<IEnumerable<Flight>> SearchAsync(SearchFlightRequest request)
        {
            return await db.Flights
                .AsNoTracking()
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a!.Configuration)
                .Include(f => f.Route!)
                    .ThenInclude(r => r.OriginAirport)
                .Include(f => f.Route!)
                    .ThenInclude(r => r.DestinationAirport)
                .Include(f => f.FlightPrices)
                .Include(f => f.Seats)
                .Include(f => f.Airline)
                .Where(f =>
                    (f.Route!.OriginAirport.IataCode == request.From || f.Route.OriginAirport.City == request.From) &&
                    (f.Route!.DestinationAirport.IataCode == request.To || f.Route.DestinationAirport.City == request.To) &&
                    f.DepartureTime.Date == request.DepartureDate.Date)
                .ToListAsync();
        }

        public async Task<Flight?> GetByFlightNumberAndDateAsync(string flightNumber, DateTime date)
        {
            return await db.Flights
                .AsNoTracking()
                .Include(f => f.Aircraft)
                .Include(f => f.Airline)
                .Include(f => f.Route)
                    .ThenInclude(r => r!.OriginAirport)
                .Include(f => f.Route)
                    .ThenInclude(r => r!.DestinationAirport)
                .Include(f => f.Statuses)
                .FirstOrDefaultAsync(f => f.FlightNumber == flightNumber &&
               f.DepartureTime.Date == date.Date);

        }

        public async Task AddAsync(Flight flight) => await db.Flights.AddAsync(flight);

        public void UpdateAsync(Flight flight) => db.Flights.Update(flight);

        public void DeleteAsync(Flight flight) => db.Flights.Remove(flight);

        public async Task SaveChangesAsync() => await db.SaveChangesAsync();
    }
}
