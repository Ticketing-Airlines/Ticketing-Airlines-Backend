﻿namespace Airline_Ticketing.Model
{
    public class AdminModel
    {
        public int AdminID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
