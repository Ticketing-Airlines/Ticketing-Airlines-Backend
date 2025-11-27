using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using Airline1.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline1.Services
{
    public class FlightPriceService(IFlightPriceRepository repo, IMapper mapper) : IFlightPriceService
    {
        // Admin Function: Creates a new price record, automatically expiring the currently active one.
        public async Task<FlightPriceResponse> CreateAsync(CreateFlightPriceRequest req)
        {
            var when = req.EffectiveFrom ?? DateTime.UtcNow;

            // 1. Find and expire the active price for this combination
            var activePrice = await repo.GetActivePriceAsync(req.FlightId, req.CabinClass, req.FlightBundleId, when);

            if (activePrice != null)
            {
                activePrice.EffectiveTo = when.AddTicks(-1);
                await repo.UpdateAsync(activePrice);
            }

            // 2. Create the new price record
            var entity = mapper.Map<FlightPrice>(req);
            entity.EffectiveFrom = when;

            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightPriceResponse>(entity);
        }

        // Admin Function: Updates properties (like price or future dates).
        public async Task<FlightPriceResponse?> UpdateAsync(int id, UpdateFlightPriceRequest req)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (req.BasePrice.HasValue) existing.BasePrice = req.BasePrice.Value;
            if (req.EffectiveFrom.HasValue) existing.EffectiveFrom = req.EffectiveFrom.Value;
            if (req.EffectiveTo.HasValue) existing.EffectiveTo = req.EffectiveTo.Value;

            existing.UpdatedBy = req.UpdatedBy ?? existing.UpdatedBy;
            existing.Note = req.Note ?? existing.Note;

            await repo.UpdateAsync(existing);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightPriceResponse>(existing);
        }

        // Admin Function: Deletes a price record.
        public async Task<bool> DeleteAsync(int id)
        {
            await repo.DeleteAsync(id);
            await repo.SaveChangesAsync();
            return true;
        }

        // Admin Function: Get all price records (history and future).
        public async Task<IEnumerable<FlightPriceResponse>> GetHistoryAsync(int flightId)
        {
            var list = await repo.GetAllByFlightAsync(flightId);
            return mapper.Map<IEnumerable<FlightPriceResponse>>(list);
        }

        // System/User Function: Get single record by ID.
        public async Task<FlightPriceResponse?> GetByIdAsync(int id)
        {
            var p = await repo.GetByIdAsync(id);
            return p == null ? null : mapper.Map<FlightPriceResponse>(p);
        }

        // System/User Function: Retrieve the active base price for a specific bundle.
        public async Task<FlightPriceResponse?> GetCurrentPriceAsync(int flightId, string cabinClass, int flightBundleId, DateTime? when = null)
        {
            var t = when ?? DateTime.UtcNow;
            var activePrice = await repo.GetActivePriceAsync(flightId, cabinClass, flightBundleId, t);
            return activePrice == null ? null : mapper.Map<FlightPriceResponse>(activePrice);
        }

        // System/User Function: Retrieve all active prices (used for UI).
        public async Task<IEnumerable<FlightPriceResponse>> GetActivePricesByFlightAsync(int flightId)
        {
            var activePrices = await repo.GetActivePricesByFlightAsync(flightId);
            return mapper.Map<IEnumerable<FlightPriceResponse>>(activePrices);
        }
    }
}