using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController(IFlightService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var flights = await service.GetAllAsync();
            return Ok(flights);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus(
            [FromQuery] string flightNumber,
            [FromQuery] DateTime date)
        {
            var flight = await service.GetFlightStatusAsync(flightNumber, date);

            if (flight == null)
            {
                return NotFound($"Sorry, we couldn't find flight {flightNumber} on that date.");
            }

            return Ok(flight);
        }

        [HttpPost("status")]
        public async Task<IActionResult> GetFlightStatus([FromBody] FlightStatusLookupRequest request)
        {
            try
            {
                var result = await service.GetFlightStatusLookupAsync(request.FlightNumber, request.Date);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new FlightStatusApiResponse
                {
                    Success = false,
                    Error = "SYSTEM_ERROR",
                    Message = "Unable to retrieve flight status. Please try again later."
                });
            }
        }

        // Legacy GET search - kept for backward compatibility
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] DateTime date,
            [FromQuery] int passengers = 1)
        {
            var flights = await service.SearchFlightsAsync(origin, destination, date, passengers);
            return Ok(flights);
        }

        // New POST search for frontend alignment
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchFlightRequest request)
        {
            var result = await service.SearchFlightsAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var flight = await service.GetByIdAsync(id);
            return flight == null ? NotFound() : Ok(flight);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlightRequest request)
        {
            var created = await service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightRequest request)
        {
            var updated = await service.UpdateAsync(id, request);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
