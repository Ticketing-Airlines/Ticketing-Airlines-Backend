using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class FlightSeat
    {
        public int LayoutID { get; set; }

        [Key]

        public int FlightSeatID { get; set; }

        public string Status { get; set; }

        public int FlightID { get; set; }
    }
}
