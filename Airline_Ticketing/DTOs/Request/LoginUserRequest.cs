using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.DTOs.Request
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        public string Password { get; set; }
    }
}
