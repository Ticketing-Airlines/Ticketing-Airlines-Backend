using Airline1.Dtos.Responses;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface ISeatSaleConfigService
    {
        Task<SeatSaleConfigResponse?> GetConfigAsync();
    }
}
