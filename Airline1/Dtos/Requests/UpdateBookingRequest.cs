using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    // Request to update a booking's status or contact info
    public class UpdateBookingRequest
    {
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [Phone]
        public string? ContactPhone { get; set; }

        // For status updates (e.g., Admin marking as Confirmed or Cancelled)
        [MaxLength(50)]
        public string? Status { get; set; }
    }
}