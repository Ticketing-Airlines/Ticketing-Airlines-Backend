using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Role { get; set; } = "Customer"; // Customer, Admin

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; } // Male, Female

        // --- New fields for session-based auth ---
        public string? SessionToken { get; set; }
        public DateTime? SessionTokenExpiry { get; set; }

        // --- For password recovery flow ---
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        // --- Status + audit fields ---
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
