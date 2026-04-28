using Airline1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.IRepositories
{
    public interface ISeatSaleRepository
    {
        Task<SeatSale?> GetByIdAsync(Guid id);
        Task<IEnumerable<SeatSale>> GetAllAsync();
        Task<IEnumerable<SeatSale>> GetActiveDealsAsync(string? type, decimal? priceMin, decimal? priceMax, bool? featured, string? sortBy);
        Task<SeatSale> AddAsync(SeatSale seatSale);
        Task UpdateAsync(SeatSale seatSale);
        Task SaveChangesAsync();
    }
}
