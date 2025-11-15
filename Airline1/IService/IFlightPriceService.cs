using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IFlightPriceService
    {
        Task<FlightPriceResponse> CreateAsync(CreateFlightPriceRequest req);
        Task<FlightPriceResponse?> UpdateAsync(int id, UpdateFlightPriceRequest req);
        Task<bool> DeleteAsync(int id);
        Task<FlightPriceResponse?> GetByIdAsync(int id);
        Task<IEnumerable<FlightPriceResponse>> GetHistoryAsync(int flightId, string cabinClass);
        Task<FlightPriceResponse?> GetCurrentPriceAsync(int flightId, string cabinClass, DateTime? when = null);
    }
}
