using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class SeatService(ISeatRepository repo, IMapper mapper) : ISeatService
    {
        // ... (Existing methods: CreateAsync, DeleteAsync, GetByIdAsync, GetByAircraftAsync, UpdateAsync) ...

        // Existing methods for context (restated for completeness)
        public async Task<SeatResponse> CreateAsync(CreateSeatRequest request)
        {
            var siblings = await repo.GetByAircraftAsync(request.AircraftId);
            if (siblings.Any(s => s.SeatNumber.Equals(request.SeatNumber, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Seat {request.SeatNumber} already exists for aircraft {request.AircraftId}.");

            var seat = mapper.Map<Seat>(request);
            var created = await repo.AddAsync(seat);
            return mapper.Map<SeatResponse>(created);
        }

        public async Task DeleteAsync(Guid id)
        {
            var seat = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Seat {id} not found.");
            await repo.DeleteAsync(seat);
        }

        public async Task<SeatResponse> GetByIdAsync(Guid id)
        {
            var seat = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Seat {id} not found.");
            return mapper.Map<SeatResponse>(seat);
        }

        public async Task<IEnumerable<SeatResponse>> GetByAircraftAsync(int aircraftId)
        {
            var seats = await repo.GetByAircraftAsync(aircraftId);
            return seats.Select(s => mapper.Map<SeatResponse>(s));
        }

        public async Task<SeatResponse> UpdateAsync(Guid id, UpdateSeatRequest request)
        {
            var seat = await repo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Seat {id} not found.");

            if (!seat.SeatNumber.Equals(request.SeatNumber, StringComparison.OrdinalIgnoreCase))
            {
                var siblings = await repo.GetByAircraftAsync(seat.AircraftId);
                if (siblings.Any(s => s.Id != id && s.SeatNumber.Equals(request.SeatNumber, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException($"Seat number {request.SeatNumber} already exists on aircraft {seat.AircraftId}.");
            }

            mapper.Map(request, seat);
            var updated = await repo.UpdateAsync(seat);
            return mapper.Map<SeatResponse>(updated);
        }

        // ---------------------------------------------------------------------
        // NEW IMPLEMENTATION FOR BULK DELETION
        // ---------------------------------------------------------------------
        public async Task DeleteAllSeatsForAircraftAsync(int aircraftId)
        {
            // 1. Retrieve all seat entities for the aircraft
            var seats = await repo.GetByAircraftAsync(aircraftId);

            if (seats.Any())
            {
                // **CRITICAL PRODUCTION CHECK (BUSINESS LOGIC)**: 
                // Ensure no seats are currently booked or occupied before deleting the configuration.
                // NOTE: This assumes the Seat model has an 'IsAvailable' property or similar booking status.
                if (seats.Any(s => !s.IsAvailable)) // Assuming 'IsAvailable' is false when booked
                {
                    throw new InvalidOperationException(
                        $"Cannot delete seats for aircraft {aircraftId}. One or more seats are currently booked."
                    );
                }

                // 2. Perform the bulk delete operation
                await repo.DeleteRangeAsync(seats);

                // 3. Persist the changes to the database
                await repo.SaveChangesAsync();
            }
            // If seats is empty, the method exits gracefully.
        }
    }
}