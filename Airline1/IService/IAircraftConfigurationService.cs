using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IAircraftConfigurationService
    {
        Task<IEnumerable<AircraftConfigurationResponse>> GetAllAsync();
        Task<AircraftConfigurationResponse?> GetByIdAsync(string id);
        Task<AircraftConfigurationResponse> CreateAsync(CreateAircraftConfigurationRequest request);
        Task<bool> UpdateAsync(string id, UpdateAircraftConfigurationRequest request);
        Task<bool> DeleteAsync(string id);
    }
}
