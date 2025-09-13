namespace Airline_Ticketing.Model
{
    public class TicketsModel
    {
        public int BookingID { get; set; }

        public int TicketID { get; set; }

        public int BPID { get; set; }

        public int TicketNumber { get; set; }

        public DateOnly IssueDate { get; set; }

        public string Status { get; set; }

        public int FlightID { get; set; }
    }
}
