using Microsoft.EntityFrameworkCore;
using Airline1.Models;

namespace Airline1.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        // Core domain sets (Keep the existing DbSet definitions)
        public DbSet<Airport> Airports { get; set; } = null!;
        public DbSet<Aircraft> Aircrafts { get; set; } = null!;
        public DbSet<FlightRoute> FlightRoutes { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<FlightSeat> FlightSeats { get; set; } = null!;
        public DbSet<Flight> Flights { get; set; } = null!;
        public DbSet<AddOnPrice> AddOnPrices { get; set; } = null!;
        public DbSet<FlightBundle> FlightBundles { get; set; } = null!;
        public DbSet<Passenger> Passengers { get; set; } = null!;
        public DbSet<AircraftConfiguration> AircraftConfigurations { get; set; } = null!;
        public DbSet<CabinConfigurationDetail> CabinConfigurationDetails { get; set; } = null!;

        public DbSet<FlightAddOn> FlightAddOns { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<BookingPassenger> BookingPassengers { get; set; } = null!;
        public DbSet<Seat> Seats { get; set; } = null!;

        // Supporting domain sets
        public DbSet<FlightStatus> FlightStatuses { get; set; } = null!;
        public DbSet<FlightStatusReason> FlightStatusReasons { get; set; } = null!;
        public DbSet<FlightPrice> FlightPrices { get; set; } = null!; // Included the DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- FLIGHT PRICE CONFIGURATION (CRITICAL UPDATE) ---
            modelBuilder.Entity<FlightPrice>(b =>
            {
                // 1. Relationship to Flight (existing)
                b.HasOne(p => p.Flight)
                    .WithMany(f => f.FlightPrices)
                    .HasForeignKey(p => p.FlightId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relationship to FlightBundle (NEW)
                b.HasOne(p => p.FlightBundle)
                    .WithMany() // Assuming FlightBundle does not have a direct collection of FlightPrices
                    .HasForeignKey(p => p.FlightBundleId)
                    .OnDelete(DeleteBehavior.Restrict); // Keep bundle if prices are deleted

                // 3. Temporal Index Update (REPLACES old index with 'Type')
                // Index is now on FlightId, CabinClass, FlightBundleId, and EffectiveFrom
                b.HasIndex(p => new { p.FlightId, p.CabinClass, p.FlightBundleId, p.EffectiveFrom })
                    .HasDatabaseName("IX_FlightPrice_Temporal_Bundle");

                // 4. Ensure price column has correct precision
                b.Property(p => p.BasePrice).HasPrecision(18, 2);
            });
            // --- END FLIGHT PRICE CONFIGURATION ---


            // --- FlightSeat Configuration (Existing, kept for completeness) ---
            modelBuilder.Entity<FlightSeat>(b =>
            {
                b.HasKey(fs => fs.FlightSeatId);
                b.Property(fs => fs.SeatClass).HasMaxLength(50);
                b.Property(fs => fs.Status).HasMaxLength(50);

                b.HasOne(fs => fs.Flight)
                   .WithMany()
                   .HasForeignKey(fs => fs.FlightId)
                   .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(fs => fs.Seat)
                   .WithMany()
                   .HasForeignKey(fs => fs.SeatId)
                   .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(fs => new { fs.FlightId, fs.SeatId })
                   .IsUnique()
                   .HasDatabaseName("IX_FlightSeat_Flight_Seat");
            });
            // --- END FlightSeat Configuration ---

            // --- Seat Configuration (Existing) ---
            modelBuilder.Entity<Seat>(b =>
            {
                b.HasIndex(s => new { s.AircraftId, s.SeatNumber })
                     .IsUnique()
                     .HasDatabaseName("IX_Seat_Aircraft_SeatNumber");

                b.Property(s => s.SeatNumber).HasMaxLength(10);
                b.Property(s => s.SeatClass).HasMaxLength(50);

                b.HasOne(s => s.Aircraft)
                    .WithMany()
                    .HasForeignKey(s => s.AircraftId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // --- END Seat Configuration ---

            // --- ADDON PRICE CONFIGURATION (Existing, kept for completeness) ---
            modelBuilder.Entity<AddOnPrice>(b =>
            {
                b.HasKey(ap => ap.AddOnPriceId);
                b.Property(ap => ap.PriceAmount).HasPrecision(18, 2);

                b.HasIndex(ap => new { ap.FlightId, ap.AddOnId, ap.ValidFrom })
                     .IsUnique()
                     .HasDatabaseName("IX_AddOnPrice_Flight_AddOn_Effective");

                b.HasOne(ap => ap.Flight)
                     .WithMany()
                     .HasForeignKey(ap => ap.FlightId)
                     .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ap => ap.AddOn)
                     .WithMany()
                     .HasForeignKey(ap => ap.AddOnId)
                     .OnDelete(DeleteBehavior.Restrict);
            });
            // --- END ADDON PRICE CONFIGURATION ---


            // --- Other Sanity Constraints (Modified to remove obsolete line) ---

            // REMOVED OBSOLETE INDEX LINE:
            // modelBuilder.Entity<FlightPrice>().HasIndex(p => new { p.FlightId, p.CabinClass, p.Type, p.EffectiveFrom }); 

            // ... (All other existing configurations remain the same) ...

            modelBuilder.Entity<Airport>()
                 .HasIndex(a => a.IataCode);

            modelBuilder.Entity<Aircraft>()
               .HasIndex(a => a.TailNumber)
               .IsUnique();

            modelBuilder.Entity<Aircraft>()
                .HasIndex(a => a.RegistrationNumber)
                .IsUnique(false);

            modelBuilder.Entity<Aircraft>()
                .HasOne(a => a.BaseAirport)
                .WithMany()
                .HasForeignKey(a => a.BaseAirportId)
                .OnDelete(DeleteBehavior.SetNull);

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

            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Statuses)
                .WithOne(s => s.Flight!)
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FlightStatus>()
                .HasOne(s => s.Reason)
                .WithMany()
                .HasForeignKey(s => s.ReasonId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<FlightStatus>()
                .HasIndex(s => new { s.FlightId, s.EffectiveAt });

            modelBuilder.Entity<FlightStatusReason>()
                .HasIndex(r => r.Code)
                .IsUnique();

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

            modelBuilder.Entity<AircraftConfiguration>()
                .HasMany(ac => ac.CabinDetails)
                .WithOne()
                .HasForeignKey(cd => cd.ConfigurationID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.Booking)
                .WithMany()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingCode)
                .IsUnique();

            modelBuilder.Entity<BookingPassenger>()
                .HasIndex(bp => new { bp.FlightId, bp.SeatNumber })
                .IsUnique();

            modelBuilder.Entity<FlightAddOn>()
                .HasIndex(a => a.Code)
                .IsUnique();

            modelBuilder.Entity<FlightAddOn>()
                .Property(p => p.Name).HasMaxLength(50);

            modelBuilder.Entity<FlightAddOn>()
                .Property(p => p.Code).HasMaxLength(20);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // --- End Other Sanity Constraints ---

            base.OnModelCreating(modelBuilder);
        }
    }
}