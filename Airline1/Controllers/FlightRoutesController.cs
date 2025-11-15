using Microsoft.AspNetCore.Mvc;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FlightRoutesController(IFlightRouteService service) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlightRouteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<FlightRouteResponse>>> GetAll()
        {
            var list = await service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FlightRouteResponse>> GetById(int id)
        {
            var item = await service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightRouteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await service.UpdateAsync(id, request);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
