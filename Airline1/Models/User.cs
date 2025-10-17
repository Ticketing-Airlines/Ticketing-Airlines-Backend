using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        [Required, MaxLength(100)]
        public required string Email { get; set; }

        [MaxLength(20)]
        public required string PhoneNumber { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } = "Customer"; // Customer, Admin

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; } // Male, Female

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
