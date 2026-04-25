using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    // Request to update a booking's contact info only
    public class UpdateBookingRequest
    {
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [Phone]
        public string? ContactPhone { get; set; }
    }
}