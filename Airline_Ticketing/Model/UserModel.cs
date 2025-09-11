namespace Airline_Ticketing.Model
{
    public class UserModel
    {

        public int UserID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public  string Password { get; set; }

        public int Phone { get; set; }

        public DateTime  CreatedAt { get; set; }
    }
}
