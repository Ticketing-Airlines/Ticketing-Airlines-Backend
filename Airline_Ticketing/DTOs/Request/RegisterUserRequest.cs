using System.ComponentModel.DataAnnotations;


namespace Airline_Ticketing.DTOs.Request
{
    public class RegisterUserRequest

    {


        [Required(ErrorMessage = "Name is Required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        public string Phone { get; set; }
    }

    
}

