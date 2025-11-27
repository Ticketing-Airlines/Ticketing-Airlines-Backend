using Airline1.IService;

namespace Airline1.Services
{
    public class FlightCostService(IFlightPriceService flightPriceService, IAddOnPriceService addOnPriceService) : IFlightCostService
    {
        public async Task<decimal> CalculateCostAsync(
            int flightId,
            string cabinClass,
            int flightBundleId,
            IEnumerable<int> addOnPriceIds)
        {
            decimal totalCost = 0m;

            // 1. Get Base Flight Price (for the selected bundle and cabin class)
            var basePriceResponse = await flightPriceService.GetCurrentPriceAsync(
                flightId,
                cabinClass,
                flightBundleId) ?? throw new ApplicationException($"No active base price found for FlightId {flightId}, CabinClass {cabinClass}, and BundleId {flightBundleId}.");
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