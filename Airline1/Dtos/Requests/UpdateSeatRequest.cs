using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class UpdateSeatRequest
    {
        [MaxLength(10)]
        public string SeatNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string SeatClass { get; set; } = "Economy";

        public bool IsExitRow { get; set; }
        public bool IsAvailable { get; set; }
    }
}
