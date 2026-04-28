namespace Airline1.Dtos.Requests
{
    public class SearchFlightRequest
    {
        public required string From { get; set; }
        public required string To { get; set; }
        public required DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int Passengers { get; set; } = 1;
        public string TripType { get; set; } = "one-way";
    }
}
