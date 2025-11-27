using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightAddOnService(IFlightAddOnRepository repo, IMapper mapper) : IFlightAddOnService
    {
        public async Task<IEnumerable<FlightAddOnResponse>> GetAllAsync()
        {
            var items = await repo.GetAllAsync();
            return items.Select(i => mapper.Map<FlightAddOnResponse>(i));
        }

        public async Task<IEnumerable<FlightAddOnResponse>> GetByCategoryAsync(AddOnCategory category)
        {
            var items = await repo.GetByCategoryAsync(category);
            return items.Select(i => mapper.Map<FlightAddOnResponse>(i));
        }

        public async Task<FlightAddOnResponse?> GetByIdAsync(int id)
        {
            var item = await repo.GetByIdAsync(id);
            return item == null ? null : mapper.Map<FlightAddOnResponse>(item);
        }

        public async Task<FlightAddOnResponse> CreateAsync(CreateFlightAddOnRequest request)
        {
            // Unique code check
            var existing = await repo.GetByCodeAsync(request.Code);
            if (existing != null)
                throw new InvalidOperationException($"Add-on code '{request.Code}' already exists.");

            var entity = mapper.Map<FlightAddOn>(request);
            entity.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightAddOnResponse>(entity);
        }

        public async Task<FlightAddOnResponse?> UpdateAsync(int id, UpdateFlightAddOnRequest request)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return null;

            // if code changed, ensure uniqueness
            if (!string.Equals(existing.Code, request.Code, StringComparison.OrdinalIgnoreCase))
            {
                var byCode = await repo.GetByCodeAsync(request.Code);
                if (byCode != null && byCode.Id != id)
                    throw new InvalidOperationException($"Add-on code '{request.Code}' already exists.");
            }

            mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateAsync(existing);
            await repo.SaveChangesAsync();

            return mapper.Map<FlightAddOnResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return false;

            await repo.DeleteAsync(id);
            await repo.SaveChangesAsync();
            return true;
        }
    }
}
