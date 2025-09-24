namespace Airline_Ticketing.Model
{
    public class Payments
    {

        public int PaymentID { get; set; }

        public int BookingID { get; set; }

        public decimal Amount { get; set; }

        public string  PaymentMethod { get; set; }

        public int TransactionID { get; set; }

        public DateOnly PaymentDate { get; set; }

        public string Status { get; set; }

        public string Currency { get; set; }
    }
}
