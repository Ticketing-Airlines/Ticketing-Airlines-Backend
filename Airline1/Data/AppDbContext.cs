using Microsoft.EntityFrameworkCore;
using Airline1.Models;

namespace Airline1.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }
        public DbSet<FlightRoute> FlightRoutes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // small sanity constraints
            modelBuilder.Entity<Airport>()
                .HasIndex(a => a.IataCode);

            modelBuilder.Entity<Aircraft>()
               .HasIndex(a => a.TailNumber)
               .IsUnique();

            modelBuilder.Entity<Aircraft>()
                .HasIndex(a => a.RegistrationNumber)
                .IsUnique(false); // registration optional uniqueness; you can make this unique if desired

            // Aircraft-BaseAirport relationship (optional FK)
            modelBuilder.Entity<Aircraft>()
                .HasOne(a => a.BaseAirport)
                .WithMany()
                .HasForeignKey(a => a.BaseAirportId)
                .OnDelete(DeleteBehavior.SetNull);

            // FlightRoute
            modelBuilder.Entity<FlightRoute>()
                .HasIndex(r => new { r.OriginAirportId, r.DestinationAirportId })
                .IsUnique();

            modelBuilder.Entity<FlightRoute>()
                .HasOne(r => r.OriginAirport)
                .WithMany()
                .HasForeignKey(r => r.OriginAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlightRoute>()
                .HasOne(r => r.DestinationAirport)
                .WithMany()
                .HasForeignKey(r => r.DestinationAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Flight relationships
            modelBuilder.Entity<Flight>()
                .Property(f => f.Price)
                .HasPrecision(18, 4);
        

            modelBuilder.Entity<Flight>()
                .HasIndex(f => f.FlightNumber)
                .IsUnique();

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Aircraft)
                .WithMany()
                .HasForeignKey(f => f.AircraftId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Route)
                .WithMany()
                .HasForeignKey(f => f.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            //  User 
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
