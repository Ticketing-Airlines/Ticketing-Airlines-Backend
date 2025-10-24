namespace Airline1.Dtos.Responses
{
    public class AuthResponse
    {
        public required int UserId { get; set; }
        public required string Email { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public string? SessionToken { get; set; }
        public DateTime? SessionExpiry { get; set; }

        // Optional convenience property (not stored in DB)
        public string FullName =>
            string.Join(" ", new[] { FirstName, MiddleName, LastName }
                .Where(name => !string.IsNullOrWhiteSpace(name)));
    }
}
