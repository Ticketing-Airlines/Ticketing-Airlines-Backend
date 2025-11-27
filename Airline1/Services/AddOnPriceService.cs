using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;
using System.Collections.Generic;

namespace Airline1.Services
{
    public class AddOnPriceService(
        IAddOnPriceRepository priceRepo,
        IFlightAddOnRepository addOnRepo,
        IMapper mapper) : IAddOnPriceService
    {
        public async Task<AddOnPriceResponse?> GetByIdAsync(int id)
        {
            var item = await priceRepo.GetByIdAsync(id);
            return item == null ? null : mapper.Map<AddOnPriceResponse>(item);
        }

        public async Task<decimal?> GetCurrentPriceAsync(int flightId, int addOnId)
        {
            var priceEntity = await priceRepo.GetActivePriceByFlightAndAddOnIdAsync(flightId, addOnId);
            return priceEntity?.PriceAmount;
        }

        public async Task<AddOnPriceResponse> CreateAsync(CreateAddOnPriceRequest request)
        {
            // --- 1. Validation Checks ---

            // Check if Add-on exists (Dependency check)
            _ = await addOnRepo.GetByIdAsync(request.AddOnId) ?? throw new InvalidOperationException($"Add-on with ID {request.AddOnId} not found.");

            // Temporal Integrity Check
            if (request.ValidTo.HasValue && request.ValidFrom > request.ValidTo.Value)
                throw new InvalidOperationException("ValidFrom date cannot be after ValidTo date.");

            // Rule Overlap Prevention
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
        {
            var existing = await priceRepo.GetByIdAsync(id);
            if (existing == null) return null;

            // --- 1. Validation Checks ---

            // Temporal Integrity Check
            if (request.ValidTo.HasValue && request.ValidFrom > request.ValidTo.Value)
                throw new InvalidOperationException("ValidFrom date cannot be after ValidTo date.");

            // Rule Overlap Prevention (Exclude the rule being updated from the overlap check)
            var overlappingRules = await priceRepo.GetOverlappingRulesAsync(
                request.FlightId, request.AddOnId, request.ValidFrom, request.ValidTo, id);

            if (overlappingRules.Any())
                throw new InvalidOperationException("Updated price rule overlaps with another existing rule. Please adjust the dates.");

            // --- 2. Update and Persistence ---

            // Important: Do not update CreatedAt, only UpdatedAt
            mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await priceRepo.UpdateAsync(existing);
            await priceRepo.SaveChangesAsync();

            return mapper.Map<AddOnPriceResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await priceRepo.GetByIdAsync(id);
            if (existing == null) return false;

            await priceRepo.DeleteAsync(id);
            await priceRepo.SaveChangesAsync();
            return true;
        }
    }
}