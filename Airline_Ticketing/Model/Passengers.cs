
using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class Passengers
    {
        [Key]
        public int PassengerID { get; set; }

        public int? UserID { get; set; }

        public String Name { get; set; }


        public DateOnly DateOfBirth { get; set; }

        public String? PassportNumber { get; set; }

        public String? Nationality { get; set; }

    }
}