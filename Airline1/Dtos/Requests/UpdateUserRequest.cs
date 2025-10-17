using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class UpdateUserRequest
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [EmailAddress, MaxLength(200)]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? Password { get; set; } // Optional: only update if provided

        [MaxLength(20)]
        public string? Role { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool? IsActive { get; set; }
    }
}
