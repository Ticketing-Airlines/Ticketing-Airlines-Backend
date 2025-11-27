using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.IServices;
using Airline_Ticketing.Model;

namespace Airline_Ticketing.Service
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request)
        {
            // Convert DTO to Model
            var newTicket = new Tickets
            {
                BookingID = request.BookingID,
                BPID = request.BPID,
                FlightID = request.FlightID,
                TicketNumber = GenerateTicketNumber(),
                IssueDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "Issued"
            };

            // Save to database using repository
            var createdTicket = await _ticketRepository.AddAsync(newTicket);

            // Convert Model to Response DTO
            return new TicketResponse
            {
                TicketID = createdTicket.TicketID,
                BookingID = createdTicket.BookingID,
                BPID = createdTicket.BPID,
                TicketNumber = createdTicket.TicketNumber,
                IssueDate = createdTicket.IssueDate,
                Status = createdTicket.Status,
                FlightID = createdTicket.FlightID
            };
        }

        public async Task<IEnumerable<TicketResponse>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();

            return tickets.Select(t => new TicketResponse
            {
                TicketID = t.TicketID,
                BookingID = t.BookingID,
                BPID = t.BPID,
                TicketNumber = t.TicketNumber,
                IssueDate = t.IssueDate,
                Status = t.Status,
                FlightID = t.FlightID
            });
        }

        public async Task<TicketResponse?> GetTicketByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
            {
                return null;
            }

            return new TicketResponse
            {
                TicketID = ticket.TicketID,
                BookingID = ticket.BookingID,
                BPID = ticket.BPID,
                TicketNumber = ticket.TicketNumber,
                IssueDate = ticket.IssueDate,
                Status = ticket.Status,
                FlightID = ticket.FlightID
            };
        }

        public async Task<TicketResponse?> UpdateTicketAsync(int id, UpdateTicketRequest request)
        {
            // Find existing ticket
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
            {
                return null;
            }

            // Update only the Status
            if (request.Status != null)
            {
                ticket.Status = request.Status;
            }

            // Save changes to database
            var updatedTicket = await _ticketRepository.UpdateAsync(ticket);

            // Return updated ticket as response DTO
            return new TicketResponse
            {
                TicketID = updatedTicket.TicketID,
                BookingID = updatedTicket.BookingID,
                BPID = updatedTicket.BPID,
                TicketNumber = updatedTicket.TicketNumber,
                IssueDate = updatedTicket.IssueDate,
                Status = updatedTicket.Status,
                FlightID = updatedTicket.FlightID
            };
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            return await _ticketRepository.DeleteAsync(id);
        }

        // Helper method to generate ticket number
        private int GenerateTicketNumber()
        {
            // Generate random 6-digit ticket number
            return new Random().Next(100000, 999999);
        }
    }
}
