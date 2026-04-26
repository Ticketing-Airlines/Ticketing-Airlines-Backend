using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IPassengerService
    {
        Task<IEnumerable<PassengerResponse>> GetAllAsync();
        Task<PassengerResponse?> GetByIdAsync(string id);
        Task<PassengerResponse> CreateAsync(CreatePassengerRequest request);
        Task<PassengerResponse?> UpdateAsync(string id, CreatePassengerRequest request);
        Task<bool> DeleteAsync(string id);
    }
}
