using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.DTOs.Request;

namespace Airline_Ticketing.IServices
{
    public interface IPassengerService
    {
        Task<IEnumerable<PassengerResponse>> GetAllPassengersAsync();
        Task<PassengerResponse?> GetPassengerByIdAsync(int id);
        Task<PassengerResponse> CreatePassengerAsync(CreatePassengerRequest request);
        Task<PassengerResponse?> UpdatePassengerAsync(int id, UpdatePassengerRequest request);


        //error:  Task<PassengerResponse?> UpdatePassengerAsync(int id, CreatePassengerRequest request);

        Task<bool> DeletePassengerAsync(int id);
    }
}