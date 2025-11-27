using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Airline1.IService
{
    public interface IFlightPriceService
    {
        // Admin: Temporal Management (CRUD)
        Task<FlightPriceResponse> CreateAsync(CreateFlightPriceRequest req);
        Task<FlightPriceResponse?> UpdateAsync(int id, UpdateFlightPriceRequest req);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FlightPriceResponse>> GetHistoryAsync(int flightId);
        Task<FlightPriceResponse?> GetByIdAsync(int id);

        // System/User: Lookup Functions

        /// <summary>
        /// Retrieves the currently active base price for a specific Flight, Cabin, and Bundle ID.
        /// </summary>
        Task<FlightPriceResponse?> GetCurrentPriceAsync(int flightId, string cabinClass, int flightBundleId, DateTime? when = null);

        /// <summary>
        /// Retrieves all currently active base prices for every unique combination on a flight (for UI display).
        /// </summary>
        Task<IEnumerable<FlightPriceResponse>> GetActivePricesByFlightAsync(int flightId);
    }
}