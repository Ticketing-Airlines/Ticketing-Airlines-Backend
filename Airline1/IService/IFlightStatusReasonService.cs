using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightStatusReasonService
    {
        Task<IEnumerable<FlightStatusReasonResponse>> GetAllAsync();
        Task<FlightStatusReasonResponse?> GetByIdAsync(int id);
        Task<FlightStatusReasonResponse?> GetByCodeAsync(string code);
        Task<FlightStatusReasonResponse> CreateAsync(FlightStatusReasonCreateRequest request);
        Task<FlightStatusReasonResponse?> UpdateAsync(int id, FlightStatusReasonUpdateRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
