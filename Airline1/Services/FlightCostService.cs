using Airline1.IService;

namespace Airline1.Services
{
    public class FlightCostService(IFlightPriceService flightPriceService, IAddOnPriceService addOnPriceService) : IFlightCostService
    {
        public async Task<decimal> CalculateCostAsync(
            int flightId,
            string cabinClass,
            int flightBundleId,
            string passengerType, 
            IEnumerable<int> addOnPriceIds)
        {
            decimal totalCost = 0m;
            var now = DateTime.UtcNow; // Set the effective date

            // 1. Get Base Flight Price (for the selected bundle and cabin class)
            var basePriceResponse = await flightPriceService.GetCurrentPriceAsync(
                flightId,
                cabinClass,
                flightBundleId,
                passengerType, 
                now) ?? throw new ApplicationException($"No active base price found for FlightId {flightId}, CabinClass {cabinClass}, BundleId {flightBundleId}, and Type {passengerType}.");

            // Add Base Price + Bundle Increment (assuming BasePriceResponse contains total base price or you calculate increment here)
            // Based on the BookingService logic, BasePriceResponse likely contains the FlightPrice details.
            // If the bundle increment is already included in basePriceResponse.BasePrice, use it directly.
            totalCost += basePriceResponse.BasePrice;

            // 2. Get Total Add-On Price (sum of selected extras)
            if (addOnPriceIds != null && addOnPriceIds.Any())
            {
                var addOnCost = await addOnPriceService.GetTotalCostByIdsAsync(addOnPriceIds);
                totalCost += addOnCost;
            }

            return totalCost;
        }
    }
}