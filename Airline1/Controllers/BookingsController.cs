using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;

        /// <summary>
        /// Creates a new flight booking (Initiates PendingPayment status).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            try
            {
                var response = await _bookingService.CreateAsync(request);
                return CreatedAtAction(nameof(GetBookingByPnr), new { pnr = response.Pnr }, response);
            }
            catch (System.Collections.Generic.KeyNotFoundException ex) // <-- FIXED
            {
                return NotFound(new { error = ex.Message });
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets a booking by its PNR (Record Locator).
        /// </summary>
        [HttpGet("{pnr}")]
        public async Task<IActionResult> GetBookingByPnr(string pnr)
        {
            var booking = await _bookingService.GetByPnrAsync(pnr);
            return booking == null ? NotFound() : Ok(booking);
        }

        /// <summary>
        /// Updates the contact information for a booking.
        /// </summary>
        [HttpPut("{pnr}")]
        public async Task<IActionResult> UpdateBookingContact(string pnr, [FromBody] UpdateBookingRequest request)
        {
            try
            {
                var updated = await _bookingService.UpdateContactInfoAsync(pnr, request);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// ADMIN/System: Updates the status of a booking (e.g., from PendingPayment to Confirmed).
        /// </summary>
        [HttpPut("{pnr}/status/{newStatus}")]
        public async Task<IActionResult> UpdateBookingStatus(string pnr, string newStatus)
        {
            try
            {
                var updated = await _bookingService.UpdateStatusAsync(pnr, newStatus);
                return updated == null ? NotFound() : Ok(updated);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gets all bookings for a specific user ID.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUserId(int userId)
        {
            var bookings = await _bookingService.GetByUserIdAsync(userId);
            return Ok(bookings);
        }

        /// <summary>
        /// Pre-calculates the price for a potential booking.
        /// </summary>
        [HttpPost("calculate-cost")]
        public async Task<IActionResult> CalculateBookingCost([FromBody] CreateBookingRequest request)
        {
            try
            {
                decimal totalCost = await _bookingService.CalculateTotalCostAsync(request);
                return Ok(new { TotalCost = totalCost, Currency = "PHP" });
            }
            catch (System.Collections.Generic.KeyNotFoundException ex) // <-- FIXED
            {
                return NotFound(new { error = ex.Message });
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}