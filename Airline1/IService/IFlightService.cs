using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightService
    {
        Task<IEnumerable<FlightResponse>> GetAllAsync();
        Task<FlightResponse?> GetByIdAsync(int id);
        Task<FlightResponse> CreateAsync(CreateFlightRequest request);
        Task<FlightResponse?> UpdateAsync(int id, UpdateFlightRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
