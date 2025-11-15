using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IAirportService
    {
        Task<AirportResponse> CreateAsync(CreateAirportRequest request);
        Task<List<AirportResponse>> GetAllAsync();
        Task<AirportResponse?> GetByIdAsync(int id);
        Task<AirportResponse?> UpdateAsync(int id, UpdateAirportRequest request);
        Task<bool> DeleteAsync(int id);     
    }
}
