namespace Airline1.Dtos.Requests
{
    // Update DTO: all fields optional so partial updates are supported
    public class UpdatePassengerRequest
    {
        public int? UserId { get; set; }         // optional: link to registered user

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Nationality { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? PassportExpiry { get; set; }

        public string? SpecialAssistance { get; set; }
        public bool? IsActive { get; set; }
    }
}
