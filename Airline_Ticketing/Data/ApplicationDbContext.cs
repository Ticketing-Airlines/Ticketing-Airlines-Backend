using Airline_Ticketing.Model;
using Microsoft.EntityFrameworkCore;    

namespace Airline_Ticketing.Data

    {
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<FlightModel> Flights { get; set; }
        public DbSet<BookingModel> Bookings { get; set; }
        public DbSet<PassengerModel> Passengers { get; set; }

        public DbSet<AdminModel> Admins { get; set; }

        public DbSet<BookingPassengerModel> BookingPassengers { get; set; }

       public DbSet<AircraftModel> Aircrafts { get; set; }

        public DbSet<AirportModel> Airports { get; set; }

        public DbSet<PaymentModel> Payments { get; set; }

        public DbSet<TicketsModel> Tickets { get; set; }

        public DbSet<SeatLayoutModel> SeatLayouts { get; set; }

        public DbSet<AirlineModel> Airlines { get; set; }

        public DbSet<FlightPricesModel> FlightPrices { get; set; }

        public DbSet<FlightSeatModel> FlightSeats { get; set; }





    }
}
