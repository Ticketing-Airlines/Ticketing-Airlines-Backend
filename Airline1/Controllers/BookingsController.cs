using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController(IBookingService service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
        {
            var booking = await service.CreateBookingAsync(request);
            if (booking == null) return BadRequest(new { message = "Unable to create booking." });
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var b = await service.GetByIdAsync(id);
            if (b == null) return NotFound();
            return Ok(b);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingRequest request)
        {
            var updated = await service.UpdateBookingAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }


        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var b = await service.GetByCodeAsync(code);
            if (b == null) return NotFound();
            return Ok(b);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var ok = await service.CancelBookingAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpGet("flight/{flightId}")]
        public async Task<IActionResult> GetByFlight(int flightId)
        {
            var list = await service.GetByFlightAsync(flightId);
            return Ok(list);
        }
    }
}
