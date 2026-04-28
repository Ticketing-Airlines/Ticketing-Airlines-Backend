using System.Collections.Generic;

namespace Airline1.Dtos.Responses
{
    public class CheckInErrorResponse
    {
        public required string ErrorCode { get; set; }
        public required string Message { get; set; }
        public List<string>? AvailableAlternatives { get; set; }
    }
}