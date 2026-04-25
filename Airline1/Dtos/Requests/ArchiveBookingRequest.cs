using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    /// <summary>
    /// Request to archive (cancel) a booking.
    /// Uses a string BookingId (PNR) as the unique identifier for Task 6.
    /// </summary>
    public class ArchiveBookingRequest
    {
        [Required]
        [MaxLength(6)]
        public required string BookingId { get; set; } // PNR - string unique ID for this task
    }
}
