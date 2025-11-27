using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Airline1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightAddOnController(IFlightAddOnService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(AddOnCategory category)
        {
            var list = await service.GetByCategoryAsync(category);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlightAddOnRequest request)
        {
            var created = await service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightAddOnRequest request)
        {
            var updated = await service.UpdateAsync(id, request);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
