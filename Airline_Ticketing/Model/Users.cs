

using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.Model
{
    public class Users
    {

        [Key]
        public int UserID { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }

        public  string Password { get; set; }

        public int Phone { get; set; }

        public DateTime  CreatedAt { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
