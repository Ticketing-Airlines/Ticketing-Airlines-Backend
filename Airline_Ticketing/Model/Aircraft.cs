using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class Aircraft
    {
        [Key]
        public int AircraftID { get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        public int AirlineID { get; set; }

        public string CreatedBy { get; set; }

    }
}
