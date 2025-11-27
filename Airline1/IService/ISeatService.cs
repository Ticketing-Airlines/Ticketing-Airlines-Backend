using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface ISeatService
    {
        Task<SeatResponse> CreateAsync(CreateSeatRequest request);
        Task<SeatResponse> UpdateAsync(int id, UpdateSeatRequest request);
        Task DeleteAsync(int id);
        Task<IEnumerable<SeatResponse>> GetByAircraftAsync(int aircraftId);
        Task<SeatResponse> GetByIdAsync(int id);
        Task DeleteAllSeatsForAircraftAsync(int aircraftId);

    }
}
