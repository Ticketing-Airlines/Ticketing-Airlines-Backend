namespace Airline_Ticketing.Model
{
    public class FlightPrices
    {
        public int FlightID { get; set; }

        public string CreatedBy { get; set; }

        public int PriceID { get; set; }

        public string Class { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }
    }
}
