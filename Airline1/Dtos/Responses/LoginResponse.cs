namespace Airline1.Dtos.Responses
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string Token { get; set; } // later if we add JWT
    }
}
