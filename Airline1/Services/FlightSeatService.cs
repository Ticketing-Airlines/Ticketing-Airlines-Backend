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
        AppDbContext db,
        IMapper mapper) : IFlightSeatService
    {
        private readonly IFlightSeatRepository _repo = repo;
        private readonly ISeatRepository _seatRepo = seatRepo;
        private readonly IFlightRepository _flightRepo = flightRepo;
        private readonly AppDbContext _db = db;
        private readonly IMapper _mapper = mapper;

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

            fs.Status = "Booked";
            fs.BookingId = request.BookingId;
            fs.PassengerId = request.PassengerId;
            fs.SeatAddOnId = request.SeatAddOnId;
            fs.UpdatedAt = DateTime.UtcNow;

            _db.FlightSeats.Update(fs);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            // map response
            var dto = new FlightSeatResponse
            {
                Id = fs.FlightSeatId,
                FlightId = fs.FlightId,
                SeatId = fs.SeatId,
                SeatNumber = fs.Seat?.SeatNumber ?? string.Empty,
                SeatClass = fs.SeatClass,
                Status = fs.Status,
                BookingId = fs.BookingId,
                PassengerId = fs.PassengerId,
                SeatAddOnId = fs.SeatAddOnId
            };

            return dto;
        }

        // Assign: Booked -> CheckedIn
        public async Task<FlightSeatResponse> AssignSeatAsync(int flightSeatId, AssignFlightSeatRequest request)
        {
            var fs = await _repo.GetByIdAsync(flightSeatId)
                ?? throw new KeyNotFoundException($"FlightSeat {flightSeatId} not found.");

            if (!string.Equals(fs.Status, "Booked", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Seat must be Booked before assigning (check-in).");

            fs.Status = "CheckedIn";
            fs.PassengerId = request.PassengerId;
            fs.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(fs);

            // map response
            var dto = new FlightSeatResponse
            {
                Id = fs.FlightSeatId,
                FlightId = fs.FlightId,
                SeatId = fs.SeatId,
                SeatNumber = fs.Seat?.SeatNumber ?? string.Empty,
                SeatClass = fs.SeatClass,
                Status = fs.Status,
                BookingId = fs.BookingId,
                PassengerId = fs.PassengerId,
                SeatAddOnId = fs.SeatAddOnId
            };

            return dto;
        }

        // Block: any -> Blocked (admin)
        public async Task<FlightSeatResponse> BlockSeatAsync(int flightSeatId, BlockFlightSeatRequest request)
        {
            var fs = await _repo.GetByIdAsync(flightSeatId)
                ?? throw new KeyNotFoundException($"FlightSeat {flightSeatId} not found.");

            fs.Status = "Blocked";
            fs.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(fs);

            var dto = new FlightSeatResponse
            {
                Id = fs.FlightSeatId,
                FlightId = fs.FlightId,
                SeatId = fs.SeatId,
                SeatNumber = fs.Seat?.SeatNumber ?? string.Empty,
                SeatClass = fs.SeatClass,
                Status = fs.Status,
                BookingId = fs.BookingId,
                PassengerId = fs.PassengerId,
                SeatAddOnId = fs.SeatAddOnId
            };

            return dto;
        }

        public async Task<IEnumerable<FlightSeatResponse>> GetByFlightAsync(int flightId)
        {
            var list = await _repo.GetByFlightAsync(flightId);
            return list.Select(fs => new FlightSeatResponse
            {
                Id = fs.FlightSeatId,
                FlightId = fs.FlightId,
                SeatId = fs.SeatId,
                SeatNumber = fs.Seat?.SeatNumber ?? string.Empty,
                SeatClass = fs.SeatClass,
                Status = fs.Status,
                BookingId = fs.BookingId,
                PassengerId = fs.PassengerId,
                SeatAddOnId = fs.SeatAddOnId
            });
        }

        public async Task<FlightSeatResponse> GetByIdAsync(int id)
        {
            var fs = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"FlightSeat {id} not found.");
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
                SeatAddOnId = fs.SeatAddOnId
            };
        }
    }
}
