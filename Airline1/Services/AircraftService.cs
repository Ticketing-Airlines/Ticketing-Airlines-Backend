using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class AircraftService(IAircraftRepository repo, IMapper mapper) : IAircraftService
    {
        private Task<bool> IsConfigurationIdValidAsync(string configurationId)
        {
            // Placeholder: Replace this with actual HTTP client call (e.g., GET /api/configurations/{configurationId})
            // If the external service returns 200 OK, return true. If 404 Not Found, return false.
            return Task.FromResult(true);
        }

        public async Task<AircraftResponse> CreateAsync(CreateAircraftRequest request)
        {
            // optional: check duplicate tail number
            var exists = await repo.GetByTailNumberAsync(request.TailNumber);
            if (exists != null)
                throw new InvalidOperationException($"Tail number '{request.TailNumber}' already exists.");

            // 2. CRITICAL: Validate the external ConfigurationID
            if (!await IsConfigurationIdValidAsync(request.ConfigurationID))
            {
                throw new InvalidOperationException($"Configuration ID '{request.ConfigurationID}' is invalid or not found in the external service.");
            }

            var model = mapper.Map<Aircraft>(request);
            model.CreatedAt = DateTime.UtcNow;

            var added = await repo.AddAsync(model);
            return mapper.Map<AircraftResponse>(added);
        }

        public async Task<List<AircraftResponse>> GetAllAsync()
        {
            var items = await repo.GetAllAsync();
            return [.. items.Select(i => mapper.Map<AircraftResponse>(i))];
        }

        public async Task<AircraftResponse?> GetByIdAsync(int id)
        {
            var item = await repo.GetByIdAsync(id);
            if (item == null) return null;
            return mapper.Map<AircraftResponse>(item);
        }

        public async Task<AircraftResponse?> UpdateAsync(int id, UpdateAircraftRequest request)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return null;

            // 1. CRITICAL: Validate ConfigurationID if the client is trying to change it
            if (request.ConfigurationID != null)
            {
                if (!await IsConfigurationIdValidAsync(request.ConfigurationID))
                {
                    throw new InvalidOperationException($"Configuration ID '{request.ConfigurationID}' is invalid or not found in the external service.");
                }
            }

            // Map non-null members from request => existing
            mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateAsync(existing);
            return mapper.Map<AircraftResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return false;
            await repo.DeleteAsync(existing);
            return true;
        }
    }
}
