namespace Airline_Ticketing.Model
{
    public class FlightModel
    {

        public int AircraftID { get; set; }

        public int FlightID { get; set; }

        public int FlightNumber { get; set; }

        public int OriginAirportID { get; set; }

        public int DestinationAirportID { get; set; }

        public string CreatedBy { get; set; }

        public TimeOnly DepartureTime { get; set; }

        public TimeOnly ArrivalTime { get; set; }

        public string Status { get; set; }
    }
}
