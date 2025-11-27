using Airline_Ticketing.Data;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.Model;
using Microsoft.EntityFrameworkCore;


namespace Airline_Ticketing.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tickets>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<Tickets?> GetByIdAsync(int id)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == id);


        }

        public async Task<Tickets> AddAsync(Tickets ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Tickets> UpdateAsync (Tickets ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketID == id);

            if(ticket == null)
            {
                return false;
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
