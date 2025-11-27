using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightBundleService
    {
        Task<IEnumerable<FlightBundleResponse>> GetAllAsync();
        Task<FlightBundleResponse?> GetByIdAsync(int id);
        Task<FlightBundleResponse> CreateAsync(CreateFlightBundleRequest request);
        Task<FlightBundleResponse?> UpdateAsync(int id, UpdateFlightBundleRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
