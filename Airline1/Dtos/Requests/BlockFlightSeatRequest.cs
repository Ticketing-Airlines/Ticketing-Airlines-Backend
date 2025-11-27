using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class BlockFlightSeatRequest
    {
        // optional reason, admin user id etc
        public string? Reason { get; set; }
    }
}
