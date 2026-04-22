using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface ISeatService
    {
        Task<SeatResponse> CreateAsync(CreateSeatRequest request);
        Task<SeatResponse> UpdateAsync(Guid id, UpdateSeatRequest request);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<SeatResponse>> GetByAircraftAsync(int aircraftId);
        Task<SeatResponse> GetByIdAsync(Guid id);
        Task DeleteAllSeatsForAircraftAsync(int aircraftId);

    }
}
