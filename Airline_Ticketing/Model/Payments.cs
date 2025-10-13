using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline_Ticketing.Model
{
    public class Payments
    {
        [Key]
        public int PaymentID { get; set; }

        public int BookingID { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string  PaymentMethod { get; set; }

        public int TransactionID { get; set; }

        public DateOnly PaymentDate { get; set; }

        public string Status { get; set; }

        public string Currency { get; set; }
    }
}
