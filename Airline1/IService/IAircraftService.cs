using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IAircraftService
    {
        Task<AircraftResponse> CreateAsync(CreateAircraftRequest request);
        Task<List<AircraftResponse>> GetAllAsync();
        Task<AircraftResponse?> GetByIdAsync(int id);
        Task<AircraftResponse?> UpdateAsync(int id, UpdateAircraftRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
