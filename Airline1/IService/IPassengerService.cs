using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IPassengerService
    {
        Task<IEnumerable<PassengerResponse>> GetAllAsync();
        Task<PassengerResponse?> GetByIdAsync(int id);
        Task<PassengerResponse> CreateAsync(CreatePassengerRequest request);
        Task<PassengerResponse?> UpdateAsync(int id, CreatePassengerRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
