namespace Airline1.Dtos.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
