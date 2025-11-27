using System.ComponentModel.DataAnnotations;
using Airline_Ticketing.Enums;

namespace Airline_Ticketing.DTOs.Request
{
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Flight ID is required.")]
        public int FlightID { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        public DateOnly BookingDate { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }
}
