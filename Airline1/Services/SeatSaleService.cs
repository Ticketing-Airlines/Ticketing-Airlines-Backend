using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;
using System.Text.Json;

namespace Airline1.Services
{
    public class SeatSaleService(
        ISeatSaleRepository seatSaleRepo,
        IMapper mapper) : ISeatSaleService
    {
        public async Task<SeatSaleActiveDealsResult> GetActiveDealsAsync(string? type, decimal? priceMin, decimal? priceMax, bool? featured, string? sortBy)
        {
            var deals = await seatSaleRepo.GetActiveDealsAsync(type, priceMin, priceMax, featured, sortBy);
            var dealsList = deals.ToList();

            if (dealsList.Count == 0)
                throw new KeyNotFoundException("No active seat sale deals available at this time");

            var response = new SeatSaleActiveDealsResult
            {
                Deals = dealsList.Select(MapToResponse).ToList(),
                Total = dealsList.Count,
                Filters = new SeatSaleAppliedFilters
                {
                    Type = type ?? "all",
                    PriceMin = priceMin,
                    PriceMax = priceMax,
                    Featured = featured
                }
            };

            return response;
        }

        public async Task<SeatSaleResponse?> GetByIdAsync(Guid id)
        {
            var seatSale = await seatSaleRepo.GetByIdAsync(id);
            return seatSale == null ? null : MapToResponse(seatSale);
        }

        private static SeatSaleResponse MapToResponse(SeatSale ss)
        {
            var discount = ss.OriginalPrice > 0
                ? (int)Math.Round(((ss.OriginalPrice - ss.SalePrice) / ss.OriginalPrice) * 100)
                : 0;

            List<string> features = [];
            try
            {
                if (!string.IsNullOrWhiteSpace(ss.Features))
                    features = JsonSerializer.Deserialize<List<string>>(ss.Features) ?? [];
            }
            catch (JsonException)
            {
                features = [];
            }

            return new SeatSaleResponse
            {
                Id = ss.Id.ToString(),
                Destination = ss.Destination,
                DestinationAirportCode = ss.DestinationAirportCode,
                Country = ss.Country,
                CountryIso2 = ss.CountryIso2,
                Type = ss.Type,
                Description = ss.Description,
                Image = ss.Image,
                Pricing = new SeatSalePricingResponse
                {
                    OriginalPrice = ss.OriginalPrice,
                    SalePrice = ss.SalePrice,
                    Discount = discount,
                    Currency = ss.Currency,
                    PriceNote = ss.PriceNote
                },
                Validity = new SeatSaleValidityResponse
                {
                    TravelPeriodStart = ss.TravelPeriodStart,
                    TravelPeriodEnd = ss.TravelPeriodEnd,
                    BookingDeadline = ss.BookingDeadline
                },
                Availability = new SeatSaleAvailabilityResponse
                {
                    SeatsLeft = ss.SeatsLeft,
                    TotalSeats = ss.TotalSeats,
                    IsLowAvailability = ss.SeatsLeft < 10
                },
                Features = features,
                Featured = ss.Featured,
                IsActive = ss.IsActive,
                CreatedAt = ss.CreatedAt,
                UpdatedAt = ss.UpdatedAt
            };
        }
    }
}
