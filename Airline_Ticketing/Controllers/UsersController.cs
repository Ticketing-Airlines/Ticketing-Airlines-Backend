using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Airline_Ticketing.Data;

namespace Airline_Ticketing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
    }
}
