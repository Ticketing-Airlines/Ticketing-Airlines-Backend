
namespace Airline1.IService
{
    public interface IFlightCostService
    {
        /// <summary>
        /// Calculates the total cost for a flight based on the selected bundle and add-ons.
        /// </summary>
        /// <param name="flightId">The ID of the flight.</param>
        /// <param name="cabinClass">The cabin class (e.g., "Economy", "Business").</param>
        /// <param name="flightBundleId">The ID of the selected flight bundle (base price).</param>
        /// <param name="addOnPriceIds">A list of selected active AddOnPrice IDs.</param>
        /// <returns>The total calculated cost.</returns>
        Task<decimal> CalculateCostAsync(
            int flightId,
            string cabinClass,
            int flightBundleId,
            string passengerType,
            IEnumerable<int> addOnPriceIds);
    }
}