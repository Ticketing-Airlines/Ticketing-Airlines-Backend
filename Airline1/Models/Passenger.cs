using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class Passenger
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey(nameof(Flight))]
        public int FlightId { get; set; } // Foreign key for the Flight
        public Flight? Flight { get; set; } // Navigation property (Fixes 'Flight' error)

        //[ForeignKey(nameof(Booking))]
        public int BookingId { get; set; } // Foreign key for the Booking/Ticket
        //public Booking? Booking { get; set; }

        [MaxLength(50)]
        public required string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Suffix { get; set; }

        public required DateTime DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(20)]
        public string? Nationality { get; set; }

        [MaxLength(20)]
        public string? PassportNumber { get; set; }

        public DateTime? PassportExpiry { get; set; }

        [MaxLength(100)]
        public string? SpecialAssistance { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    }
}
