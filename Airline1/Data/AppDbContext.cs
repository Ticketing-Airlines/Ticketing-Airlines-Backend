using Microsoft.EntityFrameworkCore;
using Airline1.Models;


namespace Airline1.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        // Core domain sets
        public DbSet<Airport> Airports { get; set; } = null!;
        public DbSet<Aircraft> Aircrafts { get; set; } = null!;
        public DbSet<FlightRoute> FlightRoutes { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<FlightSeat> FlightSeats { get; set; } = null!; 
        public DbSet<Flight> Flights { get; set; } = null!;
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
        public DbSet<FlightPrice> FlightPrices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- FlightSeat Configuration (NEW BLOCK) ---
            // Maps the many-to-many relationship between Flight and Seat for a specific instance.
            modelBuilder.Entity<FlightSeat>(b =>
            {
                b.HasKey(fs => fs.FlightSeatId);
                b.Property(fs => fs.SeatClass).HasMaxLength(50);
                b.Property(fs => fs.Status).HasMaxLength(50);

                // Relationship to Flight (required, cascade delete)
                b.HasOne(fs => fs.Flight)
                 .WithMany()
                 .HasForeignKey(fs => fs.FlightId)
                 .OnDelete(DeleteBehavior.Cascade); // If the Flight is deleted, delete the FlightSeat mapping.

                // Relationship to Seat (required, restrict delete to protect master Seat record)
                b.HasOne(fs => fs.Seat)
                 .WithMany()
                 .HasForeignKey(fs => fs.SeatId)
                 .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a Seat if it's referenced by a FlightSeat.

                // Ensure a combination of FlightId and SeatId is unique (a seat can only be on a flight once)
                b.HasIndex(fs => new { fs.FlightId, fs.SeatId })
                 .IsUnique()
                 .HasDatabaseName("IX_FlightSeat_Flight_Seat");
            });
            // --- END FlightSeat Configuration ---


            // --- Seat Configuration (Existing/Updated Block) ---
            modelBuilder.Entity<Seat>(b =>
            {
                // Ensure a seat number is unique within a single aircraft
                b.HasIndex(s => new { s.AircraftId, s.SeatNumber })
                    .IsUnique()
                    .HasDatabaseName("IX_Seat_Aircraft_SeatNumber");

                // Set property max lengths for consistency
                b.Property(s => s.SeatNumber).HasMaxLength(10);
                b.Property(s => s.SeatClass).HasMaxLength(50);

                // Define the One-to-Many relationship with Aircraft
                b.HasOne(s => s.Aircraft)
                 .WithMany() // Assuming Aircraft does not have a navigation collection for Seats, we use WithMany()
                 .HasForeignKey(s => s.AircraftId)
                 .OnDelete(DeleteBehavior.Cascade); // If the Aircraft is deleted, delete all its seats.
            });
            // --- END Seat Configuration ---


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

            // Flight -> FlightStatus (history)
            modelBuilder.Entity<Flight>()
                .HasMany(f => f.Statuses)
                .WithOne(s => s.Flight!)
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            // FlightStatus -> FlightStatusReason (optional)
            modelBuilder.Entity<FlightStatus>()
                .HasOne(s => s.Reason)
                .WithMany()
                .HasForeignKey(s => s.ReasonId)
                .OnDelete(DeleteBehavior.SetNull);

            // index to optimize queries for latest status
            modelBuilder.Entity<FlightStatus>()
                .HasIndex(s => new { s.FlightId, s.EffectiveAt });

            // Ensure FlightStatusReason
            modelBuilder.Entity<FlightStatusReason>()
                .HasIndex(r => r.Code)
                .IsUnique();

            // Flight relationships
            modelBuilder.Entity<Flight>()
                .HasMany(f => f.FlightPrices)
                .WithOne(p => p.Flight)
                .HasForeignKey(p => p.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FlightPrice>()
                .HasIndex(p => new { p.FlightId, p.CabinClass, p.Type, p.EffectiveFrom });

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
                .HasOne(p => p.Booking) // Passenger has a navigation property to Booking
                .WithMany()          // Booking does not have a collection of Passengers directly
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict); // Set to RESTRICT to avoid cycles

            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingCode)
                .IsUnique();

            modelBuilder.Entity<BookingPassenger>()
                .HasIndex(bp => new { bp.FlightId, bp.SeatNumber })
                .IsUnique();

            //FlightAddons
            modelBuilder.Entity<FlightAddOn>()
                .HasIndex(a => a.Code)
                .IsUnique();

            modelBuilder.Entity<FlightAddOn>()
                .Property(p => p.Name).HasMaxLength(50);

            modelBuilder.Entity<FlightAddOn>()
                .Property(p => p.Code).HasMaxLength(20);

            // User 
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