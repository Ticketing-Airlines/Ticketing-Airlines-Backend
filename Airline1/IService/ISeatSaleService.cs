using Airline1.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface ISeatSaleService
    {
        Task<SeatSaleActiveDealsResult> GetActiveDealsAsync(string? type, decimal? priceMin, decimal? priceMax, bool? featured, string? sortBy);
        Task<SeatSaleResponse?> GetByIdAsync(Guid id);
    }

    public class SeatSaleActiveDealsResult
    {
        public List<SeatSaleResponse> Deals { get; set; } = new();
        public int Total { get; set; }
        public SeatSaleAppliedFilters Filters { get; set; } = new();
    }

    public class SeatSaleAppliedFilters
    {
        public string Type { get; set; } = "all";
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public bool? Featured { get; set; }
    }
}
