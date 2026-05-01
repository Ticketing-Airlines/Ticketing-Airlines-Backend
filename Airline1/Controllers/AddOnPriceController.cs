using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Airline1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddOnPriceController(IAddOnPriceService addOnPriceService) : ControllerBase
    {
        // -----------------------------------------------------------
        // 1. GET: Active Price Lookup (Used by Booking Engine)
        // -----------------------------------------------------------
        /// <summary>
        /// Retrieves the currently active price for a specific add-on on a specific flight.
        /// </summary>
        /// <param name="flightId">The ID of the flight.</param>
        /// <param name="addOnId">The ID of the FlightAddOn product (e.g., Exit Row Seat).</param>
        [HttpGet("active")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetActivePrice(int flightId, int addOnId)
        {
            var price = await addOnPriceService.GetCurrentPriceAsync(flightId, addOnId);

            if (price == null)
            {
                // Return 404 if no active price rule is found for the combination
                return NotFound($"No active price found for FlightId {flightId} and AddOnId {addOnId}.");
            }

            return Ok(price.Value);
        }

        // -----------------------------------------------------------
        // 1b. GET: All Active Prices for a Flight (Used by Booking Engine)
        // -----------------------------------------------------------
        /// <summary>
        /// Retrieves all currently active add-on prices for a specific flight.
        /// </summary>
        /// <param name="flightId">The ID of the flight.</param>
        [HttpGet("flight/{flightId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AddOnPriceResponse>))]
        public async Task<IActionResult> GetActivePricesForFlight(int flightId)
        {
            var prices = await addOnPriceService.GetActivePricesForFlightAsync(flightId);
            return Ok(prices);
        }

        // -----------------------------------------------------------
        // ⭐ 2. GET: Price History (New Admin/Audit Endpoint) ⭐
        // -----------------------------------------------------------
        /// <summary>
        /// Retrieves all historical and future price rules for a specific Add-On product.
        /// </summary>
        /// <param name="addOnId">The ID of the FlightAddOn product.</param>
        [HttpGet("history/{addOnId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AddOnPriceResponse>))]
        public async Task<IActionResult> GetPriceHistory(int addOnId)
        {
            // Note: Since this fetches historical data, we return 200 OK even if the list is empty.
            var history = await addOnPriceService.GetHistoryAsync(addOnId);
            return Ok(history);
        }

        // -----------------------------------------------------------
        // 3. GET: Get Rule By ID (Admin/Audit Use)
        // -----------------------------------------------------------
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(AddOnPriceResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPriceRule(int id)
        {
            var item = await addOnPriceService.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // -----------------------------------------------------------
        // 4. POST: Create Price Rule (Admin Use)
        // -----------------------------------------------------------
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(AddOnPriceResponse))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePriceRule([FromBody] CreateAddOnPriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await addOnPriceService.CreateAsync(request);
                return CreatedAtAction(nameof(GetPriceRule), new { id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                // Catches overlap or dependency errors from the service layer
                return BadRequest(new { Error = ex.Message });
            }
        }

        // -----------------------------------------------------------
        // 5. PUT: Update Price Rule (Admin Use)
        // -----------------------------------------------------------
        [HttpPut("{id}")]
        [ProducesResponseType(200, Type = typeof(AddOnPriceResponse))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePriceRule(int id, [FromBody] UpdateAddOnPriceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await addOnPriceService.UpdateAsync(id, request);
                return response == null ? NotFound() : Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                // Catches overlap errors from the service layer
                return BadRequest(new { Error = ex.Message });
            }
        }

        // -----------------------------------------------------------
        // 6. DELETE: Delete Price Rule (Admin Use)
        // -----------------------------------------------------------
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePriceRule(int id)
        {
            var success = await addOnPriceService.DeleteAsync(id);

            // 204 No Content is the standard response for successful deletion
            return success ? NoContent() : NotFound();
        }
    }
}