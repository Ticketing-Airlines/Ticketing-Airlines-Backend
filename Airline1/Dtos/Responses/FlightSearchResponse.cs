using Airline1.Dtos.Requests;

namespace Airline1.Dtos.Responses
{
    public class FlightSearchResponse
    {
        public bool Success { get; set; } = true;
        public FlightSearchData Data { get; set; } = new();
    }

    public class FlightSearchData
    {
        public List<object> Results { get; set; } = new();
        public int TotalResults => Results.Count;
        public SearchFlightRequest SearchParams { get; set; } = null!;
    }
}
