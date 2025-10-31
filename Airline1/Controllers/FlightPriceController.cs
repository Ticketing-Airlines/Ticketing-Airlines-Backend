using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightPriceController(IFlightPriceService svc) : ControllerBase
    {
        [HttpPost] // create price or promo
        public async Task<IActionResult> Create([FromBody] CreateFlightPriceRequest req)
        {
            var created = await svc.CreateAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")] // update price/promo
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightPriceRequest req)
        {
            var updated = await svc.UpdateAsync(id, req);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await svc.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await svc.GetByIdAsync(id);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpGet("history/{flightId}/{cabinClass}")]
        public async Task<IActionResult> GetHistory(int flightId, string cabinClass)
        {
            var list = await svc.GetHistoryAsync(flightId, cabinClass);
            return Ok(list);
        }

        [HttpGet("current/{flightId}/{cabinClass}")]
        public async Task<IActionResult> GetCurrent(int flightId, string cabinClass, [FromQuery] DateTime? when = null)
        {
            var cur = await svc.GetCurrentPriceAsync(flightId, cabinClass, when);
            return cur == null ? NotFound(new { message = "No price found for flight/cabin" }) : Ok(cur);
        }
    }
}
