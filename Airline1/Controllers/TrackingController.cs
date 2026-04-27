using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackingController(ITrackingService trackingService) : ControllerBase
    {
        /// <summary>
        /// Gets the latest known GPS location for a specific IoT device.
        /// </summary>
        [HttpGet("{deviceId}/latest")]
        public async Task<IActionResult> GetLatestLocation(string deviceId)
        {
            var location = await trackingService.GetLatestLocationAsync(deviceId);
            return location == null ? NotFound(new { error = $"No location data found for device '{deviceId}'." }) : Ok(location);
        }
    }
}
