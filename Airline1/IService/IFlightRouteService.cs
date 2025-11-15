using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightRouteService
    {
        Task<FlightRouteResponse> CreateAsync(CreateFlightRouteRequest request);
        Task<List<FlightRouteResponse>> GetAllAsync();
        Task<FlightRouteResponse> GetByIdAsync(int id);
        Task<FlightRouteResponse> UpdateAsync(int id, UpdateFlightRouteRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
