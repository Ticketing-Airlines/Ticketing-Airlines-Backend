using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateAsync(CreateBookingRequest request);
        Task<BookingResponse?> GetByIdAsync(int id);
        Task<BookingResponse?> GetByPnrAsync(string pnr);
        Task<BookingResponse?> UpdateStatusAsync(string pnr, string newStatus);
        Task<BookingResponse?> UpdateContactInfoAsync(string pnr, UpdateBookingRequest request);
        Task<decimal> CalculateTotalCostAsync(CreateBookingRequest request);
        Task<IEnumerable<BookingResponse>> GetByUserIdAsync(int userId);
    }
}