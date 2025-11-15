using Airline1.Data;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Airline1.Services
{
    public class BookingService(IBookingRepository repo, IPassengerRepository passengerRepo, AppDbContext db, IMapper mapper) : IBookingService
    {

        // Helper: generate booking code
        private static string GenerateBookingCode()
        {
            var rand = RandomNumberGenerator.GetInt32(100000, 999999);
            return $"BK{DateTime.UtcNow:yyMMdd}{rand}";
        }

        public async Task<BookingResponse?> CreateBookingAsync(CreateBookingRequest request)
        {
            // validate flight exists
            var flight = await db.Flights.FindAsync(request.FlightId);
            if (flight == null) return null;

            // transaction scope
            using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                // check seat availability for each requested seat
                foreach (var p in request.Passengers)
                {
                    var seat = p.SeatNumber.Trim().ToUpper();
                    var occupied = await db.BookingPassengers
                        .AnyAsync(bp => bp.FlightId == request.FlightId && bp.SeatNumber == seat && bp.Booking.Status != BookingStatus.Cancelled);
                    if (occupied)
                        throw new InvalidOperationException($"Seat {seat} is already taken.");
                }

                var booking = new Booking
                {
                    BookingCode = GenerateBookingCode(),
                    UserId = request.UserId,
                    FlightId = request.FlightId,
                    TotalAmount = request.TotalAmount ?? 0m,
                    Status = BookingStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };

                await repo.AddAsync(booking);
                await repo.SaveChangesAsync(); // get booking.Id

                // add passengers (create guest passenger if needed)
                foreach (var p in request.Passengers)
                {
                    int? passengerId = p.PassengerId;
                    Passenger? passengerEntity = null;

                    if (passengerId.HasValue)
                    {
                        passengerEntity = await passengerRepo.GetByIdAsync(passengerId.Value);
                        if (passengerEntity == null) throw new KeyNotFoundException($"Passenger id {passengerId} not found.");
                    }
                    else
                    {
                        // create guest passenger
                        var newPassenger = new Passenger
                        {
                            UserId = null,
                            FirstName = p.FirstName ?? "Unknown",
                            MiddleName = p.MiddleName,
                            LastName = p.LastName ?? "Unknown",
                            DateOfBirth = p.DateOfBirth ?? DateTime.MinValue,
                            Email = p.Email,
                            PhoneNumber = p.PhoneNumber
                        };
                        passengerEntity = await passengerRepo.AddAsync(newPassenger);
                        await passengerRepo.SaveChangesAsync();
                    }

                    var bp = new BookingPassenger
                    {
                        BookingId = booking.Id,
                        FlightId = request.FlightId,
                        PassengerId = passengerEntity?.Id,
                        PassengerName = passengerEntity?.FullName ?? $"{p.FirstName} {p.LastName}".Trim(),
                        PassengerEmail = passengerEntity?.Email ?? p.Email,
                        SeatNumber = p.SeatNumber.Trim().ToUpper(),
                        IsContinuingPassenger = p.IsContinuingPassenger
                    };

                    db.BookingPassengers.Add(bp);
                }

                await db.SaveChangesAsync();
                await tx.CommitAsync();

                // reload full booking
                var created = await repo.GetByIdAsync(booking.Id);
                if (created == null) return null;

                // map to response
                var response = new BookingResponse
                {
                    Id = created.Id,
                    BookingCode = created.BookingCode,
                    FlightId = created.FlightId,
                    FlightNumber = created.Flight?.FlightNumber ?? string.Empty,
                    Status = created.Status,
                    TotalAmount = created.TotalAmount,
                    CreatedAt = created.CreatedAt,
                    Passengers = [.. created.Passengers.Select(bp => new BookingPassengerResponse
                    {
                        Id = bp.Id,
                        PassengerId = bp.PassengerId,
                        PassengerName = bp.PassengerName,
                        PassengerEmail = bp.PassengerEmail,
                        SeatNumber = bp.SeatNumber
                    })]
                };

                return response;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<BookingResponse?> GetByIdAsync(int id)
        {
            var b = await repo.GetByIdAsync(id);
            if (b == null) return null;
            return new BookingResponse
            {
                Id = b.Id,
                BookingCode = b.BookingCode,
                FlightId = b.FlightId,
                FlightNumber = b.Flight?.FlightNumber ?? string.Empty,
                Status = b.Status,
                TotalAmount = b.TotalAmount,
                CreatedAt = b.CreatedAt,
                Passengers = [.. b.Passengers.Select(bp => new BookingPassengerResponse
                {
                    Id = bp.Id,
                    PassengerId = bp.PassengerId,
                    PassengerName = bp.PassengerName,
                    PassengerEmail = bp.PassengerEmail,
                    SeatNumber = bp.SeatNumber
                })]
            };
        }

        public async Task<BookingResponse?> UpdateBookingAsync(int id, UpdateBookingRequest request)
        {
            var booking = await repo.GetByIdAsync(id);
            if (booking == null) return null;

            // start transaction
            using var tx = await db.Database.BeginTransactionAsync();
            try
            {
                // If total amount/status provided, update
                if (request.TotalAmount.HasValue) booking.TotalAmount = request.TotalAmount.Value;
                if (request.Status.HasValue) booking.Status = request.Status.Value;
                booking.UpdatedAt = DateTime.UtcNow;

                // If passenger list provided, we will replace existing passengers
                if (request.Passengers != null)
                {
                    // 1) Compose requested seat set (upper-case normalized)
                    var requestedSeats = request.Passengers.Select(p => p.SeatNumber.Trim().ToUpper()).ToList();

                    // 2) Check seat availability across other bookings for the same flight
                    //    exclude seats currently held by this booking (so booking can keep same seats)
                    var occupiedQuery = db.BookingPassengers
                        .Where(bp => bp.FlightId == booking.FlightId && bp.BookingId != booking.Id && bp.Booking.Status != BookingStatus.Cancelled);

                    var occupiedSeats = await occupiedQuery
                        .Select(bp => bp.SeatNumber)
                        .ToListAsync();

                    var conflicts = requestedSeats.Intersect(occupiedSeats).ToList();
                    if (conflicts.Count > 0)
                    {
                        throw new InvalidOperationException($"Seats already taken: {string.Join(", ", conflicts)}");
                    }

                    // 3) Remove existing BookingPassengers for this booking
                    var existingPassengers = booking.Passengers.ToList(); // attached entities
                    if (existingPassengers.Count > 0)
                    {
                        db.BookingPassengers.RemoveRange(existingPassengers);
                        await db.SaveChangesAsync(); // persist removal before re-adding (ensures unique index ok)
                    }

                    // 4) Add new passenger entries (create guest passenger records if needed)
                    var newBps = new List<BookingPassenger>();
                    foreach (var p in request.Passengers)
                    {
                        Passenger? passengerEntity = null;
                        if (p.PassengerId.HasValue)
                        {
                            passengerEntity = await passengerRepo.GetByIdAsync(p.PassengerId.Value);
                            if (passengerEntity == null) throw new KeyNotFoundException($"Passenger id {p.PassengerId} not found.");
                        }
                        else
                        {
                            var newPassenger = new Passenger
                            {
                                UserId = null,
                                FirstName = p.FirstName ?? "Unknown",
                                MiddleName = p.MiddleName,
                                LastName = p.LastName ?? "Unknown",
                                DateOfBirth = p.DateOfBirth ?? DateTime.MinValue,
                                Email = p.Email,
                                PhoneNumber = p.PhoneNumber
                            };
                            passengerEntity = await passengerRepo.AddAsync(newPassenger);
                            await passengerRepo.SaveChangesAsync();
                        }

                        var bp = new BookingPassenger
                        {
                            BookingId = booking.Id,
                            FlightId = booking.FlightId,
                            PassengerId = passengerEntity?.Id,
                            PassengerName = passengerEntity?.FullName ?? $"{p.FirstName} {p.LastName}".Trim(),
                            PassengerEmail = passengerEntity?.Email ?? p.Email,
                            SeatNumber = p.SeatNumber.Trim().ToUpper(),
                            IsContinuingPassenger = p.IsContinuingPassenger,
                            CreatedAt = DateTime.UtcNow
                        };

                        newBps.Add(bp);
                    }

                    // Add them to DB
                    await db.BookingPassengers.AddRangeAsync(newBps);
                }

                // persist booking updates
                repo.Update(booking);
                await repo.SaveChangesAsync();

                await tx.CommitAsync();

                // Return updated booking (reload to include passengers)
                var updated = await repo.GetByIdAsync(booking.Id);
                if (updated == null) return null;

                return new BookingResponse
                {
                    Id = updated.Id,
                    BookingCode = updated.BookingCode,
                    FlightId = updated.FlightId,
                    FlightNumber = updated.Flight?.FlightNumber ?? string.Empty,
                    Status = updated.Status,
                    TotalAmount = updated.TotalAmount,
                    CreatedAt = updated.CreatedAt,
                    Passengers = [.. updated.Passengers.Select(bp => new BookingPassengerResponse
                    {
                        Id = bp.Id,
                        PassengerId = bp.PassengerId,
                        PassengerName = bp.PassengerName,
                        PassengerEmail = bp.PassengerEmail,
                        SeatNumber = bp.SeatNumber
                    })]
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<BookingResponse?> GetByCodeAsync(string code)
        {
            var b = await repo.GetByCodeAsync(code);
            if (b == null) return null;
            return await GetByIdAsync(b.Id);
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await repo.GetByIdAsync(id);
            if (booking == null) return false;
            booking.Status = BookingStatus.Cancelled;
            await repo.CancelAsync(booking);
            return true;
        }

        public async Task<IEnumerable<BookingResponse>> GetByFlightAsync(int flightId)
        {
            var list = await repo.GetByFlightIdAsync(flightId);
            return list.Select(b => new BookingResponse
            {
                Id = b.Id,
                BookingCode = b.BookingCode,
                FlightId = b.FlightId,
                FlightNumber = b.Flight?.FlightNumber ?? string.Empty,
                Status = b.Status,
                TotalAmount = b.TotalAmount,
                CreatedAt = b.CreatedAt,
                Passengers = [.. b.Passengers.Select(bp => new BookingPassengerResponse
                {
                    Id = bp.Id,
                    PassengerId = bp.PassengerId,
                    PassengerName = bp.PassengerName,
                    PassengerEmail = bp.PassengerEmail,
                    SeatNumber = bp.SeatNumber
                })]
            });
        }
    }
}
