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
        public DbSet<Airline> Airlines { get; set; } = null!;
        public DbSet<AircraftConfiguration> AircraftConfigurations { get; set; } = null!;
        public DbSet<CabinConfigurationDetail> CabinConfigurationDetails { get; set; } = null!;
        public DbSet<FlightAddOn> FlightAddOns { get; set; } = null!;
        public DbSet<Seat> Seats { get; set; } = null!;
        public DbSet<FlightStatus> FlightStatuses { get; set; } = null!;
        public DbSet<FlightStatusReason> FlightStatusReasons { get; set; } = null!;
        public DbSet<FlightPrice> FlightPrices { get; set; } = null!;

        public DbSet<BookingFlight> BookingFlights { get; set; } = null!;
        public DbSet<TrackingDevice> TrackingDevices { get; set; } = null!;
        public DbSet<DeviceLocation> DeviceLocations { get; set; } = null!;
        public DbSet<CheckIn> CheckIns { get; set; } = null!;
        public DbSet<BoardingPass> BoardingPasses { get; set; } = null!;
        public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public DbSet<PaymentFAQ> PaymentFAQs { get; set; } = null!;

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
                .HasOne(f => f.Airline)
                .WithMany()
                .HasForeignKey(f => f.AirlineId)
                .OnDelete(DeleteBehavior.SetNull);

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

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // --- TRACKING DEVICE CONFIGURATION ---
            modelBuilder.Entity<TrackingDevice>()
                .HasKey(d => d.DeviceId);

            modelBuilder.Entity<TrackingDevice>()
                .Property(d => d.DeviceId)
                .HasMaxLength(450);

            modelBuilder.Entity<TrackingDevice>()
                .Property(d => d.MqttTopic)
                .HasMaxLength(255);

            // --- DEVICE LOCATION HISTORY CONFIGURATION ---
            modelBuilder.Entity<DeviceLocation>()
                .HasOne(dl => dl.Device)
                .WithMany()
                .HasForeignKey(dl => dl.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeviceLocation>()
                .HasIndex(dl => dl.DeviceId)
                .HasDatabaseName("IX_DeviceLocation_DeviceId");

            modelBuilder.Entity<DeviceLocation>()
                .HasIndex(dl => dl.Timestamp)
                .HasDatabaseName("IX_DeviceLocation_Timestamp");

            // --- CHECK-IN CONFIGURATION ---
            modelBuilder.Entity<CheckIn>(b =>
            {
                b.HasIndex(c => c.BookingId);
                b.HasIndex(c => c.PassengerId);
                b.HasIndex(c => c.CreatedAt).HasDatabaseName("IX_CheckIn_CreatedAt");

                b.HasOne(c => c.Booking)
                    .WithMany()
                    .HasForeignKey(c => c.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(c => c.BoardingPass)
                    .WithOne(bp => bp.CheckIn)
                    .HasForeignKey<BoardingPass>(bp => bp.CheckInId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // --- END CHECK-IN CONFIGURATION ---

            // --- BOARDING PASS CONFIGURATION ---
            modelBuilder.Entity<BoardingPass>(b =>
            {
                b.HasIndex(bp => bp.BookingReference);
                b.HasIndex(bp => bp.CheckInId).IsUnique();
            });
            // --- END BOARDING PASS CONFIGURATION ---

            // --- PAYMENT METHOD CONFIGURATION ---
            modelBuilder.Entity<PaymentMethod>(b =>
            {
                b.HasKey(pm => pm.Id);
                b.Property(pm => pm.Id).HasMaxLength(50);
                b.Property(pm => pm.Category).HasMaxLength(50);
                b.Property(pm => pm.Name).HasMaxLength(100);
                b.Property(pm => pm.Description).HasMaxLength(255);
                b.Property(pm => pm.ProcessingTime).HasMaxLength(50);
                b.Property(pm => pm.FeeType).HasMaxLength(20);
                b.Property(pm => pm.FeeCurrency).HasMaxLength(3);
                b.Property(pm => pm.FeeDisplayText).HasMaxLength(50);
                b.Property(pm => pm.Color).HasMaxLength(20);
                b.Property(pm => pm.Icon).HasMaxLength(50);
                b.Property(pm => pm.MaintenanceSchedule).HasMaxLength(255);
                b.Property(pm => pm.FeeAmount).HasPrecision(10, 2);
                b.Property(pm => pm.FeeFixedAmount).HasPrecision(10, 2);

                b.HasIndex(pm => pm.Category).HasDatabaseName("IX_PaymentMethod_Category");
                b.HasIndex(pm => pm.Featured).HasDatabaseName("IX_PaymentMethod_Featured");
                b.HasIndex(pm => pm.IsActive).HasDatabaseName("IX_PaymentMethod_IsActive");
                b.HasIndex(pm => pm.DisplayOrder).HasDatabaseName("IX_PaymentMethod_DisplayOrder");
            });
            // --- END PAYMENT METHOD CONFIGURATION ---

            // --- PAYMENT FAQ CONFIGURATION ---
            modelBuilder.Entity<PaymentFAQ>(b =>
            {
                b.HasKey(f => f.Id);
                b.Property(f => f.Category).HasMaxLength(50);
                b.Property(f => f.Question).HasMaxLength(255);
                b.Property(f => f.Icon).HasMaxLength(50);
                b.Property(f => f.Color).HasMaxLength(20);

                b.HasIndex(f => f.DisplayOrder).HasDatabaseName("IX_PaymentFAQ_DisplayOrder");
                b.HasIndex(f => f.IsActive).HasDatabaseName("IX_PaymentFAQ_IsActive");
            });
            // --- END PAYMENT FAQ CONFIGURATION ---
        }
    }
}