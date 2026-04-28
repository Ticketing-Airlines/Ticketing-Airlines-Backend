using Airline1.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatSalesController(ISeatSaleService seatSaleService, ISeatSaleConfigService configService) : ControllerBase
    {
        private readonly ISeatSaleService _seatSaleService = seatSaleService;
        private readonly ISeatSaleConfigService _configService = configService;

        /// <summary>
        /// Gets all active seat sale deals with optional filtering and sorting.
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDeals(
            [FromQuery] string? type,
            [FromQuery] decimal? priceMin,
            [FromQuery] decimal? priceMax,
            [FromQuery] bool? featured,
            [FromQuery] string? sortBy)
        {
            try
            {
                var result = await _seatSaleService.GetActiveDealsAsync(type, priceMin, priceMax, featured, sortBy);
                return Ok(new { success = true, data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = "NO_ACTIVE_DEALS", message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Gets the current seat sale configuration including terms and conditions.
        /// </summary>
        [HttpGet("config")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var result = await _configService.GetConfigAsync();
                return Ok(new { success = true, data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = "CONFIG_NOT_FOUND", message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }
}
