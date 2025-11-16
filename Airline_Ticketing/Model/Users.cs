
using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class Users
    {

            [Key]
            public int UserID { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }


        public string Email { get; set; }

            public  string Password { get; set; }

            public string Phone { get; set; }

            public DateTime  CreatedAt { get; set; }

            public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
