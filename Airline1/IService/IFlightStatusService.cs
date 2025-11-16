using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightStatusService
    {
        Task<FlightStatusResponse> CreateAsync(CreateFlightStatusRequest request);
        Task<FlightStatusResponse?> UpdateAsync(int id, UpdateFlightStatusRequest request);
        Task<FlightStatusResponse?> GetLatestByFlightIdAsync(int flightId);
        Task<IEnumerable<FlightStatusResponse>> GetHistoryByFlightIdAsync(int flightId, int limit = 100);
        Task<FlightStatusResponse?> GetByIdAsync(int id);
    }
}
