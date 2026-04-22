using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class ReserveFlightSeatRequest
    {
        public required Guid BookingId { get; set; }

        public required Guid PassengerId { get; set; }

        // optional: seat addon for pricing
        public int? SeatAddOnId { get; set; }
    }
}
