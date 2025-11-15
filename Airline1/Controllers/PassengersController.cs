using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengersController(IPassengerService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var passengers = await service.GetAllAsync();
            return Ok(passengers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var passenger = await service.GetByIdAsync(id);
            if (passenger == null) return NotFound();
            return Ok(passenger);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePassengerRequest request)
        {
            var passenger = await service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = passenger.Id }, passenger);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePassengerRequest request)
        {
            var updated = await service.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
