using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class AircraftConfigurationService(IAircraftConfigurationRepository repo, IMapper mapper) : IAircraftConfigurationService
    {
        public async Task<IEnumerable<AircraftConfigurationResponse>> GetAllAsync()
        {
            var configs = await repo.GetAllAsync();
            return mapper.Map<IEnumerable<AircraftConfigurationResponse>>(configs);
        }

        public async Task<AircraftConfigurationResponse?> GetByIdAsync(string id)
        {
            var config = await repo.GetByIdAsync(id);
            return config == null ? null : mapper.Map<AircraftConfigurationResponse>(config);
        }

        public async Task<AircraftConfigurationResponse> CreateAsync(CreateAircraftConfigurationRequest request)
        {
            var entity = mapper.Map<AircraftConfiguration>(request);
            await repo.AddAsync(entity);
            await repo.SaveChangesAsync();
            return mapper.Map<AircraftConfigurationResponse>(entity);
        }

        public async Task<bool> UpdateAsync(string id, UpdateAircraftConfigurationRequest request)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return false;

            mapper.Map(request, existing);
            await repo.UpdateAsync(existing);
            await repo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await repo.DeleteAsync(id);
            await repo.SaveChangesAsync();
            return true;
        }
    }
}
