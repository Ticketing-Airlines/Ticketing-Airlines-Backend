using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api")]
    public class FlightSeatsController(IFlightSeatService service) : ControllerBase
    {

        // Admin init
        [HttpPost("admin/flightseats/initialize/{flightId:int}")]
        public async Task<IActionResult> Initialize(int flightId)
        {
            await service.InitializeSeatsForFlightAsync(flightId);
            return Ok(new { Message = $"Flight seats initialized for flight {flightId}" });
        }

        // Reserve (booking/payment block)
        [HttpPut("flightseats/{id:int}/reserve")]
        public async Task<IActionResult> Reserve(int id, [FromBody] ReserveFlightSeatRequest request)
        {
            var res = await service.ReserveSeatAsync(id, request);
            return Ok(res);
        }

        // Assign (check-in)
        [HttpPut("flightseats/{id:int}/assign")]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignFlightSeatRequest request)
        {
            var res = await service.AssignSeatAsync(id, request);
            return Ok(res);
        }

        // Block (admin)
        [HttpPut("flightseats/{id:int}/block")]
        public async Task<IActionResult> Block(int id, [FromBody] BlockFlightSeatRequest request)
        {
            var res = await service.BlockSeatAsync(id, request);
            return Ok(res);
        }

        // Helper reads
        [HttpGet("flightseats/flight/{flightId:int}")]
        public async Task<IActionResult> GetByFlight(int flightId)
        {
            var list = await service.GetByFlightAsync(flightId);
            return Ok(list);
        }

        [HttpGet("flightseats/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await service.GetByIdAsync(id);
            return Ok(item);
        }
    }
}
