using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightStatusReasonService(IFlightStatusReasonRepository repo, IMapper mapper) : IFlightStatusReasonService
    {
        public async Task<IEnumerable<FlightStatusReasonResponse>> GetAllAsync()
        {
            var list = await repo.GetAllAsync();
            return list.Select(r => mapper.Map<FlightStatusReasonResponse>(r));
        }

        public async Task<FlightStatusReasonResponse?> GetByIdAsync(int id)
        {
            var r = await repo.GetByIdAsync(id);
            return r == null ? null : mapper.Map<FlightStatusReasonResponse>(r);
        }

        public async Task<FlightStatusReasonResponse?> GetByCodeAsync(string code)
        {
            var r = await repo.GetByCodeAsync(code);
            return r == null ? null : mapper.Map<FlightStatusReasonResponse>(r);
        }

        public async Task<FlightStatusReasonResponse> CreateAsync(FlightStatusReasonCreateRequest request)
        {
            // normalize code
            var code = request.Code.Trim().ToUpperInvariant();

            // check uniqueness
            var existing = await repo.GetByCodeAsync(code);
            if (existing != null)
                throw new InvalidOperationException($"Reason with code '{code}' already exists.");

            var entity = new FlightStatusReason
            {
                Code = code,
                Title = request.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                IsActive = request.IsActive ?? true,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightStatusReasonResponse>(entity);
        }

        public async Task<FlightStatusReasonResponse?> UpdateAsync(int id, FlightStatusReasonUpdateRequest request)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (!string.IsNullOrWhiteSpace(request.Title))
                existing.Title = request.Title.Trim();

            if (request.Description != null)
                existing.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

            if (request.IsActive.HasValue)
                existing.IsActive = request.IsActive.Value;

            existing.UpdatedAt = DateTime.UtcNow;

            repo.Update(existing);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightStatusReasonResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return false;

            // soft delete pattern could be used by toggling IsActive=false.
            // Here we perform hard delete; change if you prefer soft delete.
            repo.Remove(existing);
            await repo.SaveChangesAsync();
            return true;
        }
    }
}
