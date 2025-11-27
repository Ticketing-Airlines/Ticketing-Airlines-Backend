using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/seating-provisioning")]
    public class SeatingProvisioningController(ISeatingProvisioningService service) : ControllerBase
    {
        [HttpPost("provision/{aircraftId}")]
        public async Task<IActionResult> ProvisionSeats(int aircraftId)
        {
            await service.ProvisionSeatsForAircraftAsync(aircraftId);
            return Ok(new { Message = $"Seats provisioned for aircraft {aircraftId}." });
        }

        [HttpPost("regenerate/{aircraftId}")]
        public async Task<IActionResult> RegenerateSeats(int aircraftId)
        {
            await service.RegenerateSeatsForAircraftAsync(aircraftId);
            return Ok(new { Message = $"Seats regenerated for aircraft {aircraftId}." });
        }

    }
}
