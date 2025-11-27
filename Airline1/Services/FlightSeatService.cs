using Airline1.Data;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Services
{
    public class FlightSeatService(
        IFlightSeatRepository repo,
        ISeatRepository seatRepo,
        IFlightRepository flightRepo,
        IAddOnPriceService addOnPriceService, // <-- NEW DEPENDENCY
        AppDbContext db,
        IMapper mapper) : IFlightSeatService
    {
        private readonly IFlightSeatRepository _repo = repo;
        private readonly ISeatRepository _seatRepo = seatRepo;
        private readonly IFlightRepository _flightRepo = flightRepo;
        private readonly IAddOnPriceService _addOnPriceService = addOnPriceService; // <-- NEW FIELD
        private readonly AppDbContext _db = db;
        private readonly IMapper _mapper = mapper;

        // Private helper method to fetch price and map the final DTO
        private async Task<FlightSeatResponse> MapToResponseWithPriceAsync(FlightSeat fs)
        {
            decimal? price = null;
            string? currency = null;

            if (fs.SeatAddOnId.HasValue)
            {
                // Call the new pricing service to get the active price
                price = await _addOnPriceService.GetCurrentPriceAsync(fs.FlightId, fs.SeatAddOnId.Value);
                // NOTE: We assume PHP is the currency if a price is returned.
                // In a real system, CurrentPriceAsync would return a PriceResponse DTO with currency.
                currency = price.HasValue ? "PHP" : null;
            }

            return new FlightSeatResponse
            {
                Id = fs.FlightSeatId,
                FlightId = fs.FlightId,
                SeatId = fs.SeatId,
                SeatNumber = fs.Seat?.SeatNumber ?? string.Empty,
                SeatClass = fs.SeatClass,
                Status = fs.Status,
                BookingId = fs.BookingId,
                PassengerId = fs.PassengerId,
                SeatAddOnId = fs.SeatAddOnId,

                // --- PRICE FIELDS ADDED ---
                PriceAmount = price,
                PriceCurrency = currency
            };
        }

        // Initialize: create FlightSeat rows from Seat table for the aircraft assigned to the flight
        public async Task InitializeSeatsForFlightAsync(int flightId)
        {
            // Ensure flight exists and determine aircraft assigned
            var flight = await _flightRepo.GetByIdAsync(flightId)
                ?? throw new KeyNotFoundException($"Flight {flightId} not found.");

            var aircraftId = flight.AircraftId;

            // Get Seat template for the aircraft
            var seats = await _seatRepo.GetByAircraftAsync(aircraftId);

            // Delete any existing FlightSeat rows for idempotency
            await _repo.DeleteRangeByFlightAsync(flightId);

            var created = new List<FlightSeat>();
            foreach (var s in seats)
            {
                created.Add(new FlightSeat
                {
                    FlightId = flightId,
                    SeatId = s.Id,
                    SeatClass = s.SeatClass,
                    Status = "Available",
                    SeatAddOnId = null,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (created.Count != 0)
                await _repo.AddRangeAsync(created);
        }

        // Reserve: Available -> Booked (transactional)
        public async Task<FlightSeatResponse> ReserveSeatAsync(int flightSeatId, ReserveFlightSeatRequest request)
        {
            // Use explicit transaction to guarantee atomicity with other operations (e.g., payment)
            await using var tx = await _db.Database.BeginTransactionAsync();

            var fs = await _db.FlightSeats
                .FirstOrDefaultAsync(x => x.FlightSeatId == flightSeatId) ?? throw new KeyNotFoundException($"FlightSeat {flightSeatId} not found.");

            if (!string.Equals(fs.Status, "Available", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Seat {flightSeatId} is not Available.");

            // --- 1. Perform Seat Update ---
            fs.Status = "Booked";
            fs.BookingId = request.BookingId;
            fs.PassengerId = request.PassengerId;
            fs.SeatAddOnId = request.SeatAddOnId;
            fs.UpdatedAt = DateTime.UtcNow;

            _db.FlightSeats.Update(fs);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            // --- 2. Map Response and Include Price ---
            // Note: We need to load the 'Seat' navigation property for SeatNumber to work in MapToResponseWithPriceAsync
            // In a production repo, GetByIdAsync should include Seat, but since we used _db.FlightSeats here, we load it now.
            var fsWithSeat = await _db.FlightSeats.Include(f => f.Seat).FirstAsync(f => f.FlightSeatId == flightSeatId);

            return await MapToResponseWithPriceAsync(fsWithSeat);
        }

        // Assign: Booked -> CheckedIn
        public async Task<FlightSeatResponse> AssignSeatAsync(int flightSeatId, AssignFlightSeatRequest request)
        {
            var fs = await _repo.GetByIdAsync(flightSeatId) // Assuming GetByIdAsync includes the Seat navigation property
                ?? throw new KeyNotFoundException($"FlightSeat {flightSeatId} not found.");

            if (!string.Equals(fs.Status, "Booked", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Seat must be Booked before assigning (check-in).");

            fs.Status = "CheckedIn";
            fs.PassengerId = request.PassengerId;
            fs.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(fs);

            // Map response and Include Price
            return await MapToResponseWithPriceAsync(fs);
        }

        // Block: any -> Blocked (admin)
        public async Task<FlightSeatResponse> BlockSeatAsync(int flightSeatId, BlockFlightSeatRequest request)
        {
            var fs = await _repo.GetByIdAsync(flightSeatId)
                ?? throw new KeyNotFoundException($"FlightSeat {flightSeatId} not found.");

            fs.Status = "Blocked";
            fs.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(fs);

            // Map response and Include Price
            return await MapToResponseWithPriceAsync(fs);
        }

        // GetByFlightAsync: Seat Map Generation
        public async Task<IEnumerable<FlightSeatResponse>> GetByFlightAsync(int flightId)
        {
            var list = await _repo.GetByFlightAsync(flightId); // Get all seats for the flight
            var responses = new List<FlightSeatResponse>();

            foreach (var fs in list)
            {
                // For each seat, map to DTO and include the dynamic price
                responses.Add(await MapToResponseWithPriceAsync(fs));
            }
            return responses;
        }

        public async Task<FlightSeatResponse> GetByIdAsync(int id)
        {
            var fs = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"FlightSeat {id} not found.");

            // Map response and Include Price
            return await MapToResponseWithPriceAsync(fs);
        }
    }
}