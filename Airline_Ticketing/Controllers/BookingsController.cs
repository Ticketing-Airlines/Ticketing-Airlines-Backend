using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Airline_Ticketing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Retrieves all bookings from the system
        /// </summary>
        /// <returns>A list of all bookings</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }

        /// <summary>
        /// Retrieves a specific booking by its ID
        /// </summary>
        /// <param name="id">The booking ID to retrieve</param>
        /// <returns>The booking details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingResponse>> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = $"Booking with ID {id} was not found." });
            }

            return Ok(booking);
        }

        /// <summary>
        /// Retrieves all bookings for a specific user
        /// </summary>
        /// <param name="userId">The user ID to retrieve bookings for</param>
        /// <returns>A list of bookings for the user</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookingsByUserId(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }

        /// <summary>
        /// Creates a new booking in the system
        /// </summary>
        /// <param name="request">The booking creation request</param>
        /// <returns>The created booking</returns>
        [HttpPost]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] CreateBookingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdBooking = await _bookingService.CreateBookingAsync(request);

                return CreatedAtAction(
                    nameof(GetBookingById),
                    new { id = createdBooking.BookingID },
                    createdBooking
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        /// <param name="id">The booking ID to update</param>
        /// <param name="request">The update request</param>
        /// <returns>The updated booking</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingResponse>> UpdateBooking(int id, [FromBody] UpdateBookingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBooking = await _bookingService.UpdateBookingAsync(id, request);

            if (updatedBooking == null)
            {
                return NotFound(new { message = $"Booking with ID {id} was not found." });
            }

            return Ok(updatedBooking);
        }

        /// <summary>
        /// Deletes a booking from the system
        /// </summary>
        /// <param name="id">The booking ID to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Booking with ID {id} was not found." });
            }

            return NoContent();
        }
    }
}
