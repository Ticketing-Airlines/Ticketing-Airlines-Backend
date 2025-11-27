using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IAddOnPriceService
    {
        Task<AddOnPriceResponse> CreateAsync(CreateAddOnPriceRequest request);
        Task<AddOnPriceResponse?> GetByIdAsync(int id);
        Task<AddOnPriceResponse?> UpdateAsync(int id, UpdateAddOnPriceRequest request);
        Task<bool> DeleteAsync(int id);
        Task<decimal?> GetCurrentPriceAsync(int flightId, int addOnId);
    }
}