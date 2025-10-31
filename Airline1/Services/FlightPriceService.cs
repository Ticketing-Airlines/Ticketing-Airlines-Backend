using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightPriceService(IFlightPriceRepository repo, IMapper mapper) : IFlightPriceService
    {
        public async Task<FlightPriceResponse> CreateAsync(CreateFlightPriceRequest req)
        {
            var when = req.EffectiveFrom ?? DateTime.UtcNow;
            var type = req.Type?.Equals("Promo", StringComparison.OrdinalIgnoreCase) == true
                ? FlightPriceType.Promo : FlightPriceType.Standard;

            // Optionally expire existing standard record when creating a new standard price:
            if (type == FlightPriceType.Standard)
            {
                var activeStandard = await repo.GetActiveStandardAsync(req.FlightId, req.CabinClass, when);
                if (activeStandard != null)
                {
                    // expire it one tick before new effective start
                    activeStandard.EffectiveTo = when.AddTicks(-1);
                    await repo.UpdateAsync(activeStandard);
                }
            }

            var entity = new FlightPrice
            {
                FlightId = req.FlightId,
                CabinClass = req.CabinClass,
                BasePrice = req.BasePrice,
                Type = type,
                EffectiveFrom = req.EffectiveFrom ?? DateTime.UtcNow,
                EffectiveTo = req.EffectiveTo,
                UpdatedBy = req.UpdatedBy,
                Note = req.Note
            };

            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightPriceResponse>(entity);
        }

        public async Task<FlightPriceResponse?> UpdateAsync(int id, UpdateFlightPriceRequest req)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (req.BasePrice.HasValue) existing.BasePrice = req.BasePrice.Value;
            if (req.EffectiveFrom.HasValue) existing.EffectiveFrom = req.EffectiveFrom.Value;
            if (req.EffectiveTo.HasValue) existing.EffectiveTo = req.EffectiveTo.Value;
            existing.UpdatedBy = req.UpdatedBy ?? existing.UpdatedBy;

            await repo.UpdateAsync(existing);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightPriceResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return false;
            await repo.DeleteAsync(existing);
            await repo.SaveChangesAsync();
            return true;
        }

        public async Task<FlightPriceResponse?> GetByIdAsync(int id)
        {
            var p = await repo.GetByIdAsync(id);
            return p == null ? null : mapper.Map<FlightPriceResponse>(p);
        }

        public async Task<IEnumerable<FlightPriceResponse>> GetHistoryAsync(int flightId, string cabinClass)
        {
            var list = await repo.GetByFlightAndCabinAsync(flightId, cabinClass);
            return mapper.Map<IEnumerable<FlightPriceResponse>>(list);
        }

        public async Task<FlightPriceResponse?> GetCurrentPriceAsync(int flightId, string cabinClass, DateTime? when = null)
        {
            var t = when ?? DateTime.UtcNow;

            // 1) prefer active promo
            var promo = await repo.GetActivePromoAsync(flightId, cabinClass, t);
            if (promo != null) return mapper.Map<FlightPriceResponse>(promo);

            // 2) fallback to active standard
            var standard = await repo.GetActiveStandardAsync(flightId, cabinClass, t);
            if (standard != null) return mapper.Map<FlightPriceResponse>(standard);

            // 3) no price found
            return null;
        }
    }
}
