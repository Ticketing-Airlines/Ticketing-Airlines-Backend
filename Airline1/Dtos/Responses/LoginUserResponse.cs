namespace Airline1.Dtos.Responses
{
    public class LoginUserResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Later you can add JWT or session token here
        public string Token { get; set; } = string.Empty;
    }
}
