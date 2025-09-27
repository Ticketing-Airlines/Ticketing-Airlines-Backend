using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Services.Interfaces;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AirportsController : ControllerBase
    {
        private readonly IAirportService _service;
        public AirportsController(IAirportService service)
        {
            _service = service;
        }

        // POST api/v1/airports
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAirportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // GET api/v1/airports
        [HttpGet]
        public async Task<ActionResult<List<AirportResponse>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // GET api/v1/airports/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AirportResponse>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // PUT api/v1/airports/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAirportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE api/v1/airports/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
