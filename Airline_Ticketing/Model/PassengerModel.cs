using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class PassengerModel
    {
        public int PassengerID { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        [EmailAddress]
        public String Email { get; set; }

        public int PassPortNumber { get; set; }

    }
}
