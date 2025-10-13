using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class BookingPassengers
    {
        [Key]
        public int BPID { get; set; }
        public int BookingID { get; set; }
        public int PassengerID { get; set; }
    }
}