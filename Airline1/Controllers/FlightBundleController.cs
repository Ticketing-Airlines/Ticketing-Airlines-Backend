using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightBundleController(IFlightBundleService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var bundle = await service.GetByIdAsync(id);
            return bundle == null ? NotFound() : Ok(bundle);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlightBundleRequest request)
        {
            var result = await service.CreateAsync(request);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightBundleRequest request)
        {
            var result = await service.UpdateAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
