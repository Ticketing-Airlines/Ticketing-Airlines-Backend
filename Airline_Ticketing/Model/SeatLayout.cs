using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class SeatLayout
    {
        [Key]
        public int LayoutID { get; set; }

        public int SeatNumber { get; set; }

        public int AircraftID { get; set; }

        public string Class { get; set; }

        public string CreatedBy { get; set; }
    }
}
