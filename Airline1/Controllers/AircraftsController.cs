using Microsoft.AspNetCore.Mvc;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Services.Interfaces;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AircraftsController(IAircraftService service) : ControllerBase
    { 

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAircraftRequest request)
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
        public async Task<ActionResult<List<AircraftResponse>>> GetAll()
        {
            var list = await service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AircraftResponse>> GetById(int id)
        {
            var item = await service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAircraftRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await service.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
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
