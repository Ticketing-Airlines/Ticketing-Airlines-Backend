

namespace Airline_Ticketing.DTOs.Request
{
    public class CreatePassengerRequest
    {
       
        public string Name { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string? PassportNumber { get; set; }

        public string? Nationality { get; set; }
    }
}