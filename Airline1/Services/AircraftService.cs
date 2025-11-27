using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airline1.Services
{
    // Inject ISeatingProvisioningService for seat automation
    public class AircraftService(
        IAircraftRepository repo,
        IMapper mapper,
        ISeatingProvisioningService provisioningService) : IAircraftService
    {
        // Parameter 'config' (now named configurationId) has been removed, as requested.
        private static Task<bool> IsConfigurationIdValidAsync()
        {
            // Placeholder: Replace this with actual validation logic (e.g., calling the AircraftConfigurationService)
            return Task.FromResult(true);
        }

        public async Task<AircraftResponse> CreateAsync(CreateAircraftRequest request)
        {
            // 1. Check duplicate tail number
            var exists = await repo.GetByTailNumberAsync(request.TailNumber);
            if (exists != null)
                throw new InvalidOperationException($"Tail number '{request.TailNumber}' already exists.");

            // 2. Validate the ConfigurationID
            // The call is now simplified, but note that the validation logic will need the ID eventually.
            if (!await IsConfigurationIdValidAsync())
            {
                throw new InvalidOperationException($"Configuration ID '{request.ConfigurationID}' is invalid or not found.");
            }

            // 3. Map and Persist Aircraft
            var model = mapper.Map<Aircraft>(request);
            model.CreatedAt = DateTime.UtcNow;

            var added = await repo.AddAsync(model);
            // SaveChangesAsync is usually done inside AddAsync in a single-unit-of-work repository pattern, 
            // but if not, we must ensure the ID is generated before provisioning:
            // await repo.SaveChangesAsync(); 

            // 4. CRITICAL: Trigger initial seat generation (Orchestration)
            await provisioningService.ProvisionSeatsForAircraftAsync(added.Id);

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

            bool configurationChanged = false;

            // 1. Validate ConfigurationID if the client is trying to change it
            if (request.ConfigurationID != null && !request.ConfigurationID.Equals(existing.ConfigurationID, StringComparison.OrdinalIgnoreCase))
            {
                // The call is now simplified, but note that the validation logic will need the ID eventually.
                if (!await IsConfigurationIdValidAsync())
                {
                    throw new InvalidOperationException($"Configuration ID '{request.ConfigurationID}' is invalid or not found.");
                }
                configurationChanged = true;
            }

            // 2. Map non-null members from request => existing
            mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            // Persist the aircraft changes
            await repo.UpdateAsync(existing);

            // 3. CRITICAL: Trigger seat regeneration if configuration was updated
            if (configurationChanged)
            {
                // This method will DELETE all existing seats and create new ones based on the new configuration.
                await provisioningService.RegenerateSeatsForAircraftAsync(existing.Id);
            }

            return mapper.Map<AircraftResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return false;

            // NOTE: Deleting the aircraft should ideally cascade delete all associated seats in the database.
            await repo.DeleteAsync(existing);
            return true;
        }
    }
}