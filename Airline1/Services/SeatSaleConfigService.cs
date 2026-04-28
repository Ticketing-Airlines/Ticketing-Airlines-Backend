using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using AutoMapper;

namespace Airline1.Services
{
    public class SeatSaleConfigService(
        ISeatSaleConfigRepository seatSaleConfigRepo,
        IMapper mapper) : ISeatSaleConfigService
    {
        public async Task<SeatSaleConfigResponse?> GetConfigAsync()
        {
            var config = await seatSaleConfigRepo.GetConfigAsync();
            if (config == null) return null;

            return new SeatSaleConfigResponse
            {
                SaleEndDate = config.SaleEndDate,
                SaleTitle = config.SaleTitle,
                SaleSubtitle = config.SaleSubtitle,
                IsActive = config.IsActive,
                HeroMessage = config.HeroMessage,
                TermsAndConditions = config.TermsAndConditions.Select(tc => new TermsConditionResponse
                {
                    Id = tc.Id,
                    Icon = tc.Icon,
                    Title = tc.Title,
                    Description = tc.Description,
                    Color = tc.Color
                }).ToList(),
                Metadata = new SeatSaleMetadataResponse
                {
                    LastUpdated = config.UpdatedAt ?? config.CreatedAt,
                    Version = config.Version
                }
            };
        }
    }
}
