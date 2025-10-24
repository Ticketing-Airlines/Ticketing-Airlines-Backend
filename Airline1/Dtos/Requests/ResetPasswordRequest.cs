namespace Airline1.Dtos.Requests
{
    public class ResetPasswordRequest
    {
        public required string ResetToken { get; set; }
        public required string NewPassword { get; set; }
    }
}
