namespace Airline_Ticketing.DTOs.Response
{
    public class PassengerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }

        public string? Nationality { get; set; }

    }
}
