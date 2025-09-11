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
    }
}
