using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class CreateSeatRequest
    {
        public required int AircraftId { get; set; }

        [ MaxLength(10)]
        public required string SeatNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public required string SeatClass { get; set; } = "Economy";

        public bool IsExitRow { get; set; } = false;
    }
}
