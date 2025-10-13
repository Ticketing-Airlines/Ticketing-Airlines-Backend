using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Airline_Ticketing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly IPassengerService _passengerService;

        public PassengerController(IPassengerService passengerService)
        {
            _passengerService = passengerService;
        }

        /// <summary>
        /// Retrieves all passengers from the system
        /// </summary>
        /// <returns>A list of all passengers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PassengerResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PassengerResponse>>> GetAllPassengers()
        {
            var passengers = await _passengerService.GetAllPassengersAsync();
            return Ok(passengers);
        }

        /// <summary>
        /// Retrieves a specific passenger by their ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PassengerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PassengerResponse>> GetPassengerById(int id)
        {
            var passenger = await _passengerService.GetPassengerByIdAsync(id);

            if (passenger == null)
            {
                return NotFound(new { message = $"Passenger with ID {id} was not found." });
            }

            return Ok(passenger);
        }

        /// <summary>
        /// Creates a new passenger in the system
        /// </summary>
        /// 
        [HttpPost]
        [ProducesResponseType(typeof(PassengerResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PassengerResponse>> CreatePassenger([FromBody] CreatePassengerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPassenger = await _passengerService.CreatePassengerAsync(request);

            return CreatedAtAction(
                nameof(GetPassengerById),
                new { id = createdPassenger.Id },
                createdPassenger
            );
        }

        /// <summary>
        /// Updates an existing passenger's information
        /// </summary>
       
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PassengerResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PassengerResponse>> UpdatePassenger(int id, [FromBody] UpdatePassengerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPassenger = await _passengerService.UpdatePassengerAsync(id, request);

            if (updatedPassenger == null)
            {
                return NotFound(new { message = $"Passenger with ID {id} was not found." });
            }

            return Ok(updatedPassenger);
        }

        /// <summary>
        /// Deletes a passenger from the system
        /// </summary>
        /// <param name="id">The passenger ID to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            var result = await _passengerService.DeletePassengerAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Passenger with ID {id} was not found." });
            }

            return NoContent();
        }
    }
}