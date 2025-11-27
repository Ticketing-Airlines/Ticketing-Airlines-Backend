namespace Airline_Ticketing.DTOs.Response
{
    public class TicketResponse
    {
        public int TicketID { get; set; }

        public int BookingID { get; set; }

        public int BPID { get; set; }

        public int TicketNumber { get; set; }

        public DateOnly IssueDate { get; set; }

        public string Status { get; set; }

        public int FlightID { get; set; }
    }
}
