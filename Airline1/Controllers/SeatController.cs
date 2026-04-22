using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _service;

        public SeatController(ISeatService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSeatRequest request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var seat = await _service.GetByIdAsync(id);
            return Ok(seat);
        }

        [HttpGet("aircraft/{aircraftId:int}")]
        public async Task<IActionResult> GetByAircraft(int aircraftId)
        {
            var seats = await _service.GetByAircraftAsync(aircraftId);
            return Ok(seats);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSeatRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);
            return Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
