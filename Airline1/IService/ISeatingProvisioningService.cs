using System.Threading.Tasks;

namespace Airline1.IService
{
    public interface ISeatingProvisioningService
    {
        Task ProvisionSeatsForAircraftAsync(int aircraftId);
        Task RegenerateSeatsForAircraftAsync(int aircraftId);
    }
}
