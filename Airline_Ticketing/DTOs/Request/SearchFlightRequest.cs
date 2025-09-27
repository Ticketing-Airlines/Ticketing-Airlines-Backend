using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.DTOs.Request
{
    public class SearchFlightRequest
    {

        [Required(ErrorMessage = "Origin Airport is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Origin Airport code must be 3 characters long")]
        public string OriginAirportCode { get; set; }

        [Required(ErrorMessage = "Destination Airport is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Destination Airport code must be 3 characters long")]
        public string DestinationAirportCode { get; set; }

        
        [Required(ErrorMessage = "Departure Date is required")]
        public DateOnly DepartureDate { get; set; }


        ///  <summary>
        ///  Optional if Date is required

        ///   </summary>
        public DateOnly? ReturnDate { get; set; }

        /// <summary>
        /// The number of passengers traveling. This is needed to check for seat availability.
        /// </summary>

        [Required(ErrorMessage = "Number of passengers is required.")]
        [Range(1, 9, ErrorMessage = "Please select between 1 and 9 passengers.")]
        public int NumberOfPassengers { get; set; }


    }
}
