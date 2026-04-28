namespace Airline1.Dtos.Responses
{
    public class RoundTripCombinationResponse
    {
        public FlightSearchResultResponse Outbound { get; set; } = null!;
        public FlightSearchResultResponse Return { get; set; } = null!;
        public decimal TotalPrice { get; set; }
    }
}
