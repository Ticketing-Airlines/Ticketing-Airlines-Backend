using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.DTOs.Request;

namespace Airline_Ticketing.IServices
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketResponse>> GetAllTicketsAsync();

        Task<TicketResponse?> GetTicketByIdAsync(int id);

        Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request);

        Task<TicketResponse?> UpdateTicketAsync(int id, UpdateTicketRequest request);

        Task<bool> DeleteTicketAsync(int id);
    }
}
