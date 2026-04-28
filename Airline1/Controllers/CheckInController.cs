using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Airline1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/check-in")]
    public class CheckInController(
        ICheckInService checkInService,
        CheckInRateLimiter rateLimiter) : ControllerBase
    {
        /// <summary>
        /// Checks if a booking is eligible for online check-in.
        /// Lightweight endpoint that accepts a 6-character booking reference.
        /// </summary>
        [HttpGet("eligibility")]
        public async Task<IActionResult> GetEligibility([FromQuery] string bookingReference)
        {
            var response = await checkInService.GetEligibilityAsync(bookingReference);
            return Ok(response);
        }

        /// <summary>
        /// Verifies booking reference and last name for check-in.
        /// Rate limited to 5 attempts per IP per 15 minutes.
        /// </summary>
        [HttpPost("verify")]
        [EnableRateLimiting("Verify")]
        public async Task<IActionResult> Verify([FromBody] CheckInVerifyRequest request)
        {
            try
            {
                var response = await checkInService.VerifyAsync(request);
                if (!response.IsVerified)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new CheckInErrorResponse
                {
                    ErrorCode = "INVALID_REQUEST",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Completes the check-in process, assigns seats, generates boarding passes.
        /// Rate limited to 3 attempts per booking per hour.
        /// </summary>
        [HttpPost("complete")]
        public async Task<IActionResult> Complete([FromBody] CheckInCompleteRequest request)
        {
            // Custom rate limiting: 3 attempts per booking per hour
            if (!rateLimiter.AllowComplete(request.BookingReference))
            {
                return StatusCode(429, new CheckInErrorResponse
                {
                    ErrorCode = "RATE_LIMITED",
                    Message = "Too many check-in attempts for this booking. Please try again later."
                });
            }

            try
            {
                var response = await checkInService.CompleteAsync(request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new CheckInErrorResponse
                {
                    ErrorCode = "BOOKING_NOT_FOUND",
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("SEAT_UNAVAILABLE"))
            {
                var alternatives = ex.Message.Replace("SEAT_UNAVAILABLE:", "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                return Conflict(new CheckInErrorResponse
                {
                    ErrorCode = "SEAT_UNAVAILABLE",
                    Message = "One or more selected seats are no longer available.",
                    AvailableAlternatives = alternatives
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new CheckInErrorResponse
                {
                    ErrorCode = "INVALID_REQUEST",
                    Message = ex.Message
                });
            }
        }
    }
}
