namespace Airline1.Dtos.Requests
{
    public class RegisterUserRequest
    {
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }
}
