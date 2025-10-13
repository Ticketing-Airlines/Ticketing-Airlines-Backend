using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        public DateOnly BookingDate { get; set; }

        public int TotalAmount { get; set; }

        public string Status { get; set; }
    }
}
