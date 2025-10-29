namespace Airline_Ticketing.DTOs.Response
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; } // JWT token or session token
        public string Message { get; set; }
    }
}
