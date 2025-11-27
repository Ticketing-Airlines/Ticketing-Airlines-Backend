using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface IAddOnPriceService
    {
        // ----------------------------------------------------------------------
        // Admin/CRUD Functions (Based on your existing structure + standard requirements)
        // ----------------------------------------------------------------------
        Task<AddOnPriceResponse> CreateAsync(CreateAddOnPriceRequest request);
        Task<AddOnPriceResponse?> UpdateAsync(int id, UpdateAddOnPriceRequest request);
        Task<bool> DeleteAsync(int id);

        // This is necessary for displaying historical prices for a specific add-on
        Task<IEnumerable<AddOnPriceResponse>> GetHistoryAsync(int flightAddOnId);

        // ----------------------------------------------------------------------
        // Query Functions (Used by Booking and Cost Services)
        // ----------------------------------------------------------------------
        Task<AddOnPriceResponse?> GetByIdAsync(int id);
        Task<decimal?> GetCurrentPriceAsync(int flightId, int addOnId);

        /// <summary>
        /// Calculates the total cost for a list of active Add-On Price IDs.
        /// </summary>
        // ⭐ THE NEW METHOD ADDED FOR FLIGHT COST SERVICE ⭐
        Task<decimal> GetTotalCostByIdsAsync(IEnumerable<int> addOnPriceIds);
    }
}