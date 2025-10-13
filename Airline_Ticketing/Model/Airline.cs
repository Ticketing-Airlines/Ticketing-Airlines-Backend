using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class Airline
    {
        [Key]
        public int AirlineID { get; set; }

        public string Country { get; set; }

        public string Code { get; set; }

        public string CreatedBy { get; set; }
    }
}
