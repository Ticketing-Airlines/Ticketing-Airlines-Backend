using Airline1.Common;
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
    public class FlightStatusService(IFlightStatusRepository repo, AppDbContext db, IMapper mapper) : IFlightStatusService
    {

        // allowed transitions as per your approved table
        private static readonly Dictionary<FlightStatusType, FlightStatusType[]> AllowedTransitions =
            new()
            {
                { FlightStatusType.Scheduled, new[] { FlightStatusType.Boarding, FlightStatusType.Delayed, FlightStatusType.Cancelled } },
                { FlightStatusType.Boarding, new[] { FlightStatusType.Departed, FlightStatusType.Delayed, FlightStatusType.Cancelled } },
                { FlightStatusType.Departed, new[] { FlightStatusType.Landed, FlightStatusType.Delayed } },
                { FlightStatusType.Delayed, new[] { FlightStatusType.Boarding, FlightStatusType.Departed, FlightStatusType.Cancelled } },
                { FlightStatusType.Cancelled, Array.Empty<FlightStatusType>() }, // final
                { FlightStatusType.Landed, Array.Empty<FlightStatusType>() } // final
            };

        public async Task<FlightStatusResponse> CreateAsync(CreateFlightStatusRequest request)
        {
            // ensure flight exists - FIXED: Using discard '_' to stop 'Unnecessary assignment' warning.
            _ = await db.Flights.FindAsync(request.FlightId)
                ?? throw new KeyNotFoundException($"Flight {request.FlightId} not found.");

            // validate reason id if provided - FIXED: Using discard '_' for the same reason.
            if (request.ReasonId.HasValue)
            {
                var reasonId = request.ReasonId.Value;
                _ = await db.FlightStatusReasons.FindAsync(reasonId)
                    ?? throw new KeyNotFoundException($"Reason {reasonId} not found.");
            }

            // check last status to validate transition (if any)
            var last = await repo.GetLatestByFlightIdAsync(request.FlightId);
            if (last != null)
            {
                // if last is Cancelled or Landed, block new statuses (immutable), instruct reschedule instead
                if (last.Status == FlightStatusType.Cancelled || last.Status == FlightStatusType.Landed)
                {
                    throw new InvalidOperationException($"Flight {request.FlightId} has final status {last.Status}. Create a new flight (reschedule) instead.");
                }

                // validate allowed transition
                if (!AllowedTransitions.TryGetValue(last.Status, out var allowed) || !allowed.Contains(request.Status))
                {
                    throw new InvalidOperationException($"Transition from {last.Status} to {request.Status} is not allowed.");
                }
            }
            else
            {
                // no previous: only Scheduled allowed? we allow any first status but usually Scheduled
                // no extra check here
            }

            var entity = new FlightStatus
            {
                FlightId = request.FlightId,
                Status = request.Status,
                ReasonId = request.ReasonId,
                EffectiveAt = request.EffectiveAt ?? DateTime.UtcNow,
                UpdatedBy = string.IsNullOrWhiteSpace(request.UpdatedBy) ? "system" : request.UpdatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();

            // reload with relations
            var saved = await repo.GetByIdAsync(entity.Id);
            return mapper.Map<FlightStatusResponse>(saved!);
        }

        public async Task<FlightStatusResponse?> GetByIdAsync(int id)
        {
            var e = await repo.GetByIdAsync(id);
            return e == null ? null : mapper.Map<FlightStatusResponse>(e);
        }

        public async Task<FlightStatusResponse?> GetLatestByFlightIdAsync(int flightId)
        {
            var e = await repo.GetLatestByFlightIdAsync(flightId);
            return e == null ? null : mapper.Map<FlightStatusResponse>(e);
        }

        public async Task<IEnumerable<FlightStatusResponse>> GetHistoryByFlightIdAsync(int flightId, int limit = 100)
        {
            var list = await repo.GetHistoryByFlightIdAsync(flightId, limit);
            return list.Select(x => mapper.Map<FlightStatusResponse>(x));
        }

        public async Task<FlightStatusResponse?> UpdateAsync(int id, UpdateFlightStatusRequest request)
        {
            // Find existing record to ensure it exists, then validate transition from latest.
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return null;

            // existing identifies the flight; we use existing.FlightId to create new status record
            var flightId = existing.FlightId;

            // If existing record is final, prevent updates
            if (existing.Status == FlightStatusType.Cancelled || existing.Status == FlightStatusType.Landed)
            {
                throw new InvalidOperationException($"Cannot change final status {existing.Status} for flight {flightId}. Create a new flight for reschedule.");
            }

            // Determine new status: if request.Status provided, use it; otherwise cannot proceed
            if (!request.Status.HasValue)
            {
                throw new ArgumentException("Update must specify a new Status to append.");
            }

            // validate ReasonId if provided - FIXED: Using discard '_' for the same reason.
            if (request.ReasonId.HasValue)
            {
                var reasonId = request.ReasonId.Value;
                _ = await db.FlightStatusReasons.FindAsync(reasonId)
                    ?? throw new KeyNotFoundException($"Reason {reasonId} not found.");
            }

            // Check allowed transition based on latest
            var last = await repo.GetLatestByFlightIdAsync(flightId);
            if (last != null)
            {
                if (last.Status == FlightStatusType.Cancelled || last.Status == FlightStatusType.Landed)
                {
                    throw new InvalidOperationException($"Flight {flightId} has final status {last.Status}. Create a new flight (reschedule) instead.");
                }

                if (!AllowedTransitions.TryGetValue(last.Status, out var allowed) || !allowed.Contains(request.Status.Value))
                {
                    throw new InvalidOperationException($"Transition from {last.Status} to {request.Status.Value} is not allowed.");
                }
            }

            var newEntry = new FlightStatus
            {
                FlightId = flightId,
                Status = request.Status.Value,
                ReasonId = request.ReasonId,
                EffectiveAt = request.EffectiveAt ?? DateTime.UtcNow,
                UpdatedBy = string.IsNullOrWhiteSpace(request.UpdatedBy) ? "system" : request.UpdatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(newEntry);
            await repo.SaveChangesAsync();

            var saved = await repo.GetByIdAsync(newEntry.Id);
            return mapper.Map<FlightStatusResponse>(saved!);
        }
    }
}