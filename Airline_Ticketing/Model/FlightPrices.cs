using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline_Ticketing.Model
{
    public class FlightPrices
    {
        public int FlightID { get; set; }

        public string CreatedBy { get; set; }

        [Key]
        public int PriceID { get; set; }

        public string Class { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public string Currency { get; set; }
    }
}
