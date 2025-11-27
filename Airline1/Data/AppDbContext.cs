using Airline1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Added for completeness if needed elsewhere

namespace Airline1.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<BookingPassenger> BookingPassengers { get; set; } = null!;
        public DbSet<BookingAddOn> BookingAddOns { get; set; } = null!;
        public DbSet<FlightSeat> FlightSeats { get; set; } = null!;
        public DbSet<AddOnPrice> AddOnPrices { get; set; } = null!;
        public DbSet<Airport> Airports { get; set; } = null!;
        public DbSet<Aircraft> Aircrafts { get; set; } = null!;
        public DbSet<FlightRoute> FlightRoutes { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Flight> Flights { get; set; } = null!;
        public DbSet<FlightBundle> FlightBundles { get; set; } = null!;
        public DbSet<Passenger> Passengers { get; set; } = null!;
        public DbSet<AircraftConfiguration> AircraftConfigurations { get; set; } = null!;
        public DbSet<CabinConfigurationDetail> CabinConfigurationDetails { get; set; } = null!;
        public DbSet<FlightAddOn> FlightAddOns { get; set; } = null!;
        public DbSet<Seat> Seats { get; set; } = null!;
        public DbSet<FlightStatus> FlightStatuses { get; set; } = null!;
        public DbSet<FlightStatusReason> FlightStatusReasons { get; set; } = null!;
        public DbSet<FlightPrice> FlightPrices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- BOOKING/BOOKINGPASSENGER CONFIGURATION (FIXED) ---

            // Restored fundamental booking relationship (Assuming Booking has a Pnr property)
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.Pnr)
                .IsUnique();

            // BookingPassenger to Booking (One Booking has Many BookingPassengers)
            modelBuilder.Entity<BookingPassenger>()
                .HasOne(bp => bp.Booking) // Uses the navigation property defined in the model
                .WithMany(b => b.Passengers) // Assuming Booking model has ICollection<BookingPassenger> Passengers
                .HasForeignKey(bp => bp.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookingPassenger to FlightSeat (Seat Assignment) - 1-to-0/1
            // Uses the explicit configuration to define the nullable relationship
            modelBuilder.Entity<BookingPassenger>()
                .HasOne(bp => bp.FlightSeat)
                .WithOne(fs => fs.BookingPassenger) // Assuming FlightSeat links back to BookingPassenger
                .HasForeignKey<BookingPassenger>(bp => bp.FlightSeatId)
                .IsRequired(false) // Matches the nullable int? FlightSeatId
                .OnDelete(DeleteBehavior.Restrict);

            // --- BOOKING ADD-ON CONFIGURATION (FIXED) ---

            // CRITICAL FIX: The primary key for BookingAddOn is [Key] public int BookingAddOnId. 
            // Removed the incorrect composite key definition that was conflicting.

            // Relationship to BookingPassenger (One passenger has many add-ons)
            modelBuilder.Entity<BookingAddOn>()
                .HasOne(bao => bao.Passenger) // Uses the navigation property
                .WithMany(bp => bp.AddOns)    // Uses the inverse collection navigation property
                .HasForeignKey(bao => bao.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship to AddOnPrice (Many add-ons use one AddOnPrice rule)
            modelBuilder.Entity<BookingAddOn>()
                .HasOne(bao => bao.AddOnPrice) // Uses the navigation property
                .WithMany() // No inverse collection property defined on AddOnPrice (using convention)
                .HasForeignKey(bao => bao.AddOnPriceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Removed the redundant index on { PassengerId, AddOnPriceId } as it's not a composite key.

            // --- FLIGHT PRICE CONFIGURATION ---
            modelBuilder.Entity<FlightPrice>(b =>
            {
                // 1. Relationship to Flight (existing)
                b.HasOne(p => p.Flight)
                    .WithMany(f => f.FlightPrices)
                    .HasForeignKey(p => p.FlightId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 2. Relationship to FlightBundle (NEW)
                b.HasOne(p => p.FlightBundle)
                    .WithMany()
                    .HasForeignKey(p => p.FlightBundleId)
                    .OnDelete(DeleteBehavior.Restrict);

                // 3. Temporal Index Update 
                b.HasIndex(p => new { p.FlightId, p.CabinClass, p.FlightBundleId, p.PassengerType, p.EffectiveFrom })
                    .HasDatabaseName("IX_FlightPrice_Temporal_Bundle");

                // 4. Ensure price column has correct precision
                b.Property(p => p.BasePrice).HasPrecision(18, 2);
            });
            // --- END FLIGHT PRICE CONFIGURATION ---

            // --- FlightSeat Configuration ---
            modelBuilder.Entity<FlightSeat>(b =>
            {
                b.HasKey(fs => fs.FlightSeatId);
                b.Property(fs => fs.SeatClass).HasMaxLength(50);
                b.Property(fs => fs.Status).HasMaxLength(50);

                b.HasOne(fs => fs.Flight)
                   .WithMany(f => f.Seats) // Assuming Flight has ICollection<FlightSeat> Seats
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

            // --- ADDON PRICE CONFIGURATION ---
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


            // --- Other Sanity Constraints ---

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

            // This relationship is likely incorrect since we are using BookingPassenger now.
            // Keeping it for now but suggesting a check:
            // The new BookingPassenger model seems to replace this old Passenger model relationship.
            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.Booking)
                .WithMany()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}