using Airline1.Dtos.Responses;
using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface ITrackingService
    {
        Task<DeviceLocationResponse?> GetLatestLocationAsync(string deviceId);
    }
}
