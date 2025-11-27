using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Airline1.IRepositories
{
    public interface IFlightPriceRepository
    {
        // Standard CRUD
        Task<FlightPrice?> GetByIdAsync(int id);
        Task<IEnumerable<FlightPrice>> GetAllByFlightAsync(int flightId);
        Task AddAsync(FlightPrice price);
        Task UpdateAsync(FlightPrice price);
        Task DeleteAsync(int id);
        Task SaveChangesAsync(); // For committing temporal updates

        // Temporal Lookup Methods

        /// <summary>
        /// Retrieves the currently active FlightPrice record for a specific Flight, Cabin Class, and Bundle at time 't'.
        /// </summary>
        Task<FlightPrice?> GetActivePriceAsync(int flightId, string cabinClass, int flightBundleId, string passengerType,DateTime t);

        /// <summary>
        /// Retrieves all currently active FlightPrice records for a flight (used for presenting all bundle prices).
        /// </summary>
        Task<IEnumerable<FlightPrice>> GetActivePricesByFlightAsync(int flightId);

        /// <summary>
        /// Retrieves the single FlightPrice record that is scheduled to become active next (for validation/audit).
        /// </summary>
        Task<FlightPrice?> GetNextScheduledPriceAsync(int flightId, string cabinClass, int flightBundleId);
    }
}