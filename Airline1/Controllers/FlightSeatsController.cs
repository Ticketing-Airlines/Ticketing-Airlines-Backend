using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    // Use the component name as the base route (e.g., /api/FlightSeat)
    [Route("api/[controller]")]
    [ApiController]
    public class FlightSeatController(IFlightSeatService flightSeatService) : ControllerBase
    {
        // -----------------------------------------------------------
        // 1. GET: Seat Map Generation (Client-Facing Read)
        // -----------------------------------------------------------
        /// <summary>
        /// Retrieves the complete seat map for a flight, including status and real-time price.
        /// </summary>
        /// <param name="flightId">The ID of the flight to get the seat map for.</param>
        [HttpGet("flight/{flightId:int}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<FlightSeatResponse>))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSeatMap(int flightId)
        {
            var seatMap = await flightSeatService.GetByFlightAsync(flightId);

            if (seatMap == null || !seatMap.Any())
            {
                // Note: The service should ideally throw KeyNotFoundException if the flight itself is missing.
                return NotFound($"No seat inventory found for Flight ID {flightId}.");
            }

            return Ok(seatMap);
        }

        // -----------------------------------------------------------
        // 2. GET: Single Seat Status/Price Lookup (Helper Read)
        // -----------------------------------------------------------
        [HttpGet("{id:guid}")]
        [ProducesResponseType(200, Type = typeof(FlightSeatResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var item = await flightSeatService.GetByIdAsync(id);
                return Ok(item);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // -----------------------------------------------------------
        // 3. POST: Reserve Seat (Transactional Action)
        // -----------------------------------------------------------
        /// <summary>
        /// Reserves an available seat (Available -> Booked).
        /// </summary>
        /// <param name="flightSeatId">The ID of the specific FlightSeat.</param>
        [HttpPost("{flightSeatId:guid}/reserve")]
        [ProducesResponseType(200, Type = typeof(FlightSeatResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ReserveSeat(Guid flightSeatId, [FromBody] ReserveFlightSeatRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var response = await flightSeatService.ReserveSeatAsync(flightSeatId, request);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"FlightSeat {flightSeatId} not found.");
            }
            catch (InvalidOperationException ex)
            {
                // Catches errors like "Seat is not Available"
                return BadRequest(new { Error = ex.Message });
            }
        }

        // -----------------------------------------------------------
        // 4. POST: Assign Seat (Transactional Action)
        // -----------------------------------------------------------
        /// <summary>
        /// Assigns a seat to a passenger during check-in (Booked -> CheckedIn).
        /// </summary>
        /// <param name="flightSeatId">The ID of the specific FlightSeat.</param>
        [HttpPost("{flightSeatId:guid}/assign")]
        [ProducesResponseType(200, Type = typeof(FlightSeatResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AssignSeat(Guid flightSeatId, [FromBody] AssignFlightSeatRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var response = await flightSeatService.AssignSeatAsync(flightSeatId, request);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"FlightSeat {flightSeatId} not found.");
            }
            catch (InvalidOperationException ex)
            {
                // Catches errors like "Seat must be Booked before assigning"
                return BadRequest(new { Error = ex.Message });
            }
        }

        // -----------------------------------------------------------
        // 5. POST: Block Seat (Admin Action)
        // -----------------------------------------------------------
        /// <summary>
        /// Blocks a seat from selection for administrative reasons (Any Status -> Blocked).
        /// </summary>
        /// <param name="flightSeatId">The ID of the specific FlightSeat.</param>
        [HttpPost("{flightSeatId:guid}/block")]
        [ProducesResponseType(200, Type = typeof(FlightSeatResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Block(Guid flightSeatId, [FromBody] BlockFlightSeatRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var response = await flightSeatService.BlockSeatAsync(flightSeatId, request);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"FlightSeat {flightSeatId} not found.");
            }
        }


        // -----------------------------------------------------------
        // 6. POST: Initialize Seats (Internal/Setup Action)
        // -----------------------------------------------------------
        /// <summary>
        /// Initializes the FlightSeat inventory for a new flight.
        /// </summary>
        [HttpPost("initialize/{flightId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> InitializeSeats(int flightId)
        {
            try
            {
                await flightSeatService.InitializeSeatsForFlightAsync(flightId);
                // Standard REST response for a successful operation with no data returned
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                // Catches if the Flight/Aircraft doesn't exist
                return NotFound(new { Error = ex.Message });
            }
        }
    }
}