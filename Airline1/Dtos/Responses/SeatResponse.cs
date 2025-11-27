namespace Airline1.Dtos.Responses
{
    public class SeatResponse
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatClass { get; set; } = string.Empty;
        public bool IsExitRow { get; set; }
        public bool IsAvailable { get; set; }
    }
}
