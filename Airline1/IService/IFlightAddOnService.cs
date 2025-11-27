using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface IFlightAddOnService
    {
        Task<IEnumerable<FlightAddOnResponse>> GetAllAsync();
        Task<IEnumerable<FlightAddOnResponse>> GetByCategoryAsync(AddOnCategory category);
        Task<FlightAddOnResponse?> GetByIdAsync(int id);
        Task<FlightAddOnResponse> CreateAsync(CreateFlightAddOnRequest request);
        Task<FlightAddOnResponse?> UpdateAsync(int id, UpdateFlightAddOnRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
