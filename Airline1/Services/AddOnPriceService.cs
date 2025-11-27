using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    // Note: Added necessary 'using' statements for IEnumerable, Linq, and System/Task/DateTime
    public class AddOnPriceService(
        IAddOnPriceRepository priceRepo,
        IFlightAddOnRepository addOnRepo,
        IMapper mapper) : IAddOnPriceService
    {
        // ----------------------------------------------------------------------
        // EXISTING METHODS (Retained)
        // ----------------------------------------------------------------------

        public async Task<AddOnPriceResponse?> GetByIdAsync(int id)
        { /* ... implementation ... */
            var item = await priceRepo.GetByIdAsync(id);
            return item == null ? null : mapper.Map<AddOnPriceResponse>(item);
        }

        public async Task<decimal?> GetCurrentPriceAsync(int flightId, int addOnId)
        { /* ... implementation ... */
            var priceEntity = await priceRepo.GetActivePriceByFlightAndAddOnIdAsync(flightId, addOnId);
            return priceEntity?.PriceAmount;
        }

        public async Task<AddOnPriceResponse> CreateAsync(CreateAddOnPriceRequest request)
        { /* ... implementation ... */
            // --- 1. Validation Checks ---
            _ = await addOnRepo.GetByIdAsync(request.AddOnId) ?? throw new InvalidOperationException($"Add-on with ID {request.AddOnId} not found.");
            if (request.ValidTo.HasValue && request.ValidFrom > request.ValidTo.Value)
                throw new InvalidOperationException("ValidFrom date cannot be after ValidTo date.");

            var overlappingRules = await priceRepo.GetOverlappingRulesAsync(
                request.FlightId, request.AddOnId, request.ValidFrom, request.ValidTo);

            if (overlappingRules.Any())
                throw new InvalidOperationException("New price rule overlaps with an existing rule. Please adjust the dates.");

            // --- 2. Creation and Persistence ---
            var entity = mapper.Map<AddOnPrice>(request);
            entity.CreatedAt = DateTime.UtcNow;

            await priceRepo.AddAsync(entity);
            await priceRepo.SaveChangesAsync();

            return mapper.Map<AddOnPriceResponse>(entity);
        }

        public async Task<AddOnPriceResponse?> UpdateAsync(int id, UpdateAddOnPriceRequest request)
        { /* ... implementation ... */
            var existing = await priceRepo.GetByIdAsync(id);
            if (existing == null) return null;

            // --- 1. Validation Checks ---
            if (request.ValidTo.HasValue && request.ValidFrom > request.ValidTo.Value)
                throw new InvalidOperationException("ValidFrom date cannot be after ValidTo date.");

            var overlappingRules = await priceRepo.GetOverlappingRulesAsync(
                request.FlightId, request.AddOnId, request.ValidFrom, request.ValidTo, id);

            if (overlappingRules.Any())
                throw new InvalidOperationException("Updated price rule overlaps with another existing rule. Please adjust the dates.");

            // --- 2. Update and Persistence ---
            mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await priceRepo.UpdateAsync(existing);
            await priceRepo.SaveChangesAsync();

            return mapper.Map<AddOnPriceResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        { /* ... implementation ... */
            var existing = await priceRepo.GetByIdAsync(id);
            if (existing == null) return false;

            await priceRepo.DeleteAsync(id);
            await priceRepo.SaveChangesAsync();
            return true;
        }

        // ----------------------------------------------------------------------
        // NEW METHOD ADDED (Required by the updated interface)
        // ----------------------------------------------------------------------

        public async Task<IEnumerable<AddOnPriceResponse>> GetHistoryAsync(int flightAddOnId)
        {
            // Assuming IAddOnPriceRepository has this method (or similar)
            var list = await priceRepo.GetAllByFlightAddOnAsync(flightAddOnId);
            return mapper.Map<IEnumerable<AddOnPriceResponse>>(list);
        }

        public async Task<decimal> GetTotalCostByIdsAsync(IEnumerable<int> addOnPriceIds)
        {
            if (addOnPriceIds == null || !addOnPriceIds.Any())
            {
                return 0m;
            }

            // Requires: IAddOnPriceRepository to have a GetPricesByIdsAsync method
            var prices = await priceRepo.GetPricesByIdsAsync(addOnPriceIds);

            var now = DateTime.UtcNow;
            var activePrices = prices.Where(p =>
                p.ValidFrom <= now &&
                (p.ValidTo == null || p.ValidTo > now)
            ).ToList();

            // Sums the prices of only the currently active, valid price records found
            return activePrices.Sum(p => p.PriceAmount);
        }
    }
}