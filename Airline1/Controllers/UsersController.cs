using Microsoft.AspNetCore.Mvc;
using Airline1.Dtos.Requests;
using Airline1.IService;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var response = await userService.RegisterUserAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var response = await userService.LoginUserAsync(request);
            if (response == null) return Unauthorized("Invalid credentials");
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await userService.GetUserByIdAsync(id);
            if (response == null) return NotFound();
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await userService.GetAllUsersAsync();
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var response = await userService.UpdateUserAsync(id, request);
            if (response == null) return NotFound();
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var success = await userService.DeleteUserAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
