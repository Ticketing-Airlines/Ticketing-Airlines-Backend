using Airline1.Models;
using System.Threading.Tasks;

namespace Airline1.IRepositories
{
    public interface ISeatSaleConfigRepository
    {
        Task<SeatSaleConfig?> GetConfigAsync();
        Task<SeatSaleConfig> AddAsync(SeatSaleConfig config);
        Task UpdateAsync(SeatSaleConfig config);
        Task SaveChangesAsync();
    }
}
