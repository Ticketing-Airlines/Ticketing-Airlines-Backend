using Airline1.Dtos.Requests;
using Airline1.IService;
using Microsoft.AspNetCore.Mvc;

namespace Airline1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
                return Unauthorized("Invalid credentials.");
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader] string sessionToken)
        {
            var success = await _authService.LogoutAsync(sessionToken);
            if (!success) return BadRequest("Invalid session token.");
            return Ok("Logged out successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request);
            if (!result) return NotFound("User not found.");
            return Ok("Password reset token generated.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            if (!result) return BadRequest("Invalid or expired reset token.");
            return Ok("Password has been reset successfully.");
        }
    }
}
