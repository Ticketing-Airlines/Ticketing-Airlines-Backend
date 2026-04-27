namespace Airline1.Dtos.Responses
{
    public class LoginResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = "";
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string Token { get; set; } // later if we add JWT
    }
}
