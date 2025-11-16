using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightStatusController(IFlightStatusService service) : ControllerBase
    {

        [HttpGet("flight/{flightId:int}")]
        public async Task<IActionResult> GetLatestByFlight(int flightId)
        {
            var result = await service.GetLatestByFlightIdAsync(flightId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // Get history
        [HttpGet("flight/{flightId:int}/history")]
        public async Task<IActionResult> GetHistory(int flightId, [FromQuery] int limit = 100)
        {
            var result = await service.GetHistoryByFlightIdAsync(flightId, limit);
            return Ok(result);
        }

        // Get by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // Create new status (append)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlightStatusRequest request)
        {
            try
            {
                var created = await service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException knf)
            {
                return BadRequest(new { message = knf.Message });
            }
            catch (InvalidOperationException inv)
            {
                return BadRequest(new { message = inv.Message });
            }
        }

        // Update: appends a new status (see service comment)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightStatusRequest request)
        {
            try
            {
                var updated = await service.UpdateAsync(id, request);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (KeyNotFoundException knf)
            {
                return BadRequest(new { message = knf.Message });
            }
            catch (InvalidOperationException inv)
            {
                return BadRequest(new { message = inv.Message });
            }
            catch (ArgumentException arg)
            {
                return BadRequest(new { message = arg.Message });
            }
        }
    }
}
