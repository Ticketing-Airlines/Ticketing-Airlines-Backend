using System.ComponentModel.DataAnnotations;

namespace Airline_Ticketing.DTOs.Request
{
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "A Flight ID must be Provied.")]
        public int FlightID { get; set; }

        [Required(ErrorMessage = "At least one passenger is required to make a booking.")]
        public List<PassengerDetailsRequest> Passengers { get; set; } = new List<PassengerDetailsRequest>();


    }

    public class PassengerDetailsRequest
    {
        [Required(ErrorMessage = "Name is Required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of Your Birth is Required")]
        public DateOnly DateOfBirth { get; set; }



        [Required(ErrorMessage = "Passport Number is Required")]
        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "Your Nationality is Required")]
        [StringLength(100)]
        public string Nationality { get; set; }


        [Required(ErrorMessage = "A seat number must be selected for each passenger")]
        public string SeatNumber { get; set; }
    }
}
