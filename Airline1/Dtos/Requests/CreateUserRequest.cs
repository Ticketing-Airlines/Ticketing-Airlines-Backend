using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class CreateUserRequest
    {
        [Required, MaxLength(100)]
        public required string FirstName { get; set; }

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [Required, MaxLength(100)]
        public required string LastName { get; set; }

        [Required, EmailAddress, MaxLength(200)]
        public required string Email { get; set; }

        [Required, MinLength(6)]
        public required string Password { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Customer"; // Default role

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
    }
}
