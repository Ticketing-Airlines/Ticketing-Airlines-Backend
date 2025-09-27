using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Airline_Ticketing.Data;
using System.Reflection.Metadata.Ecma335;

namespace Airline_Ticketing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet] 
         public async Task<IActionResult> GetAllFlights()
        {
            var flights = await _context.Flights.ToListAsync();
           
            return Ok(flights);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetFlightById(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
           
            if (flight == null)
            {
                return NotFound();
            }

            return Ok(flight);
        }

        
    }
}
