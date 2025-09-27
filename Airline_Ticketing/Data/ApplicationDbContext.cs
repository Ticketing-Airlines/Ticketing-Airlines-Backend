using Airline_Ticketing.Model;
using Microsoft.EntityFrameworkCore;    

namespace Airline_Ticketing.Data

    {
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Flights> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passengers> Passengers { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<BookingPassengers> BookingPassengers { get; set; }

       public DbSet<Aircraft> Aircrafts { get; set; }

        public DbSet<Airport> Airports { get; set; }

        public DbSet<Payments> Payments { get; set; }

        public DbSet<Tickets> Tickets { get; set; }

        public DbSet<SeatLayout> SeatLayouts { get; set; }

        public DbSet<Airline> Airlines { get; set; }

        public DbSet<FlightPrices> FlightPrices { get; set; }

        public DbSet<FlightSeat> FlightSeats { get; set; }

        public DbSet<Messages> Message { get; set; }

        public DbSet<Chats> Chats { get; set; }



    }
}
