using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.DTOs.Request
{
    public class UpdatePassengerRequest
    {

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [StringLength(50, ErrorMessage = "Passport number cannot be longer than 50 characters.")]
        public string? PassportNumber { get; set; }

        [StringLength(50, ErrorMessage = "Nationality cannot be longer than 50 characters.")]
        public string? Nationality { get; set; }
    }
}
