using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Airline_Ticketing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Retrieves all tickets from the system
        /// </summary>
        /// <returns>A list of all tickets</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TicketResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        /// <summary>
        /// Retrieves a specific ticket by its ID
        /// </summary>
        /// <param name="id">The ticket ID to retrieve</param>
        /// <returns>The ticket details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TicketResponse>> GetTicketById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound(new { message = $"Ticket with ID {id} was not found." });
            }

            return Ok(ticket);
        }

        /// <summary>
        /// Creates a new ticket in the system
        /// Automatically generates TicketNumber, IssueDate, and sets Status to "Issued"
        /// </summary>
        /// <param name="request">The ticket creation request</param>
        /// <returns>The created ticket</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TicketResponse>> CreateTicket([FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTicket = await _ticketService.CreateTicketAsync(request);

            return CreatedAtAction(
                nameof(GetTicketById),
                new { id = createdTicket.TicketID },
                createdTicket
            );
        }

        /// <summary>
        /// Updates an existing ticket's status
        /// </summary>
        /// <param name="id">The ticket ID to update</param>
        /// <param name="request">The update request containing new status</param>
        /// <returns>The updated ticket</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TicketResponse>> UpdateTicket(int id, [FromBody] UpdateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedTicket = await _ticketService.UpdateTicketAsync(id, request);

            if (updatedTicket == null)
            {
                return NotFound(new { message = $"Ticket with ID {id} was not found." });
            }

            return Ok(updatedTicket);
        }

        /// <summary>
        /// Deletes a ticket from the system
        /// </summary>
        /// <param name="id">The ticket ID to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var result = await _ticketService.DeleteTicketAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Ticket with ID {id} was not found." });
            }

            return NoContent();
        }
    }
}
