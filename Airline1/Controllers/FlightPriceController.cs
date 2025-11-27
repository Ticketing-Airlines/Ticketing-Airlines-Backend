using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPriceController(IFlightPriceService service) : ControllerBase
    {
        // --- ADMIN ENDPOINTS ---

        /// <summary>
        /// Creates a new base price record for a Flight/Cabin/Bundle.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FlightPriceResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePrice([FromBody] CreateFlightPriceRequest request)
        {
            var response = await service.CreateAsync(request);
            return CreatedAtAction(nameof(GetPriceById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Updates an existing price record (e.g., changing the BasePrice or EffectiveTo date).
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FlightPriceResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdateFlightPriceRequest request)
        {
            var response = await service.UpdateAsync(id, request);
            if (response == null) return NotFound();
            return Ok(response);
        }

        /// <summary>
        /// Retrieves the entire history of prices for a flight.
        /// </summary>
        [HttpGet("History/{flightId}")]
        [ProducesResponseType(typeof(IEnumerable<FlightPriceResponse>), 200)]
        public async Task<IActionResult> GetPriceHistory(int flightId)
        {
            var response = await service.GetHistoryAsync(flightId);
            return Ok(response);
        }

        /// <summary>
        /// Deletes a price record.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePrice(int id)
        {
            var success = await service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // --- USER/SYSTEM LOOKUP ENDPOINTS ---

        /// <summary>
        /// Retrieves ALL currently active base prices for a given flight (for presenting bundle options).
        /// </summary>
        [HttpGet("Active/Flight/{flightId}")]
        [ProducesResponseType(typeof(IEnumerable<FlightPriceResponse>), 200)]
        public async Task<IActionResult> GetActivePrices(int flightId)
        {
            var response = await service.GetActivePricesByFlightAsync(flightId);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves the single active base price for a specific Flight, Cabin, and Bundle ID (used by the Cost Service).
        /// </summary>
        [HttpGet("Current")]
        [ProducesResponseType(typeof(FlightPriceResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCurrentPrice(
            [FromQuery] int flightId,
            [FromQuery] string cabinClass,
            [FromQuery] int flightBundleId,
            [FromQuery] string passengerType)
        {
            var response = await service.GetCurrentPriceAsync(flightId, cabinClass, flightBundleId, passengerType);
            if (response == null) return NotFound(new { message = "No active base price found for this combination." });
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a single price record by its unique ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FlightPriceResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPriceById(int id)
        {
            var response = await service.GetByIdAsync(id);
            if (response == null) return NotFound();
            return Ok(response);
        }
    }
}