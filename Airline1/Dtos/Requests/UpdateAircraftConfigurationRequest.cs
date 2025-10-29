namespace Airline1.Dtos.Requests
{
    public class UpdateAircraftConfigurationRequest
    {
        public string? AircraftModel { get; set; }
        public int? TotalSeats { get; set; }
        public List<CabinDetailDto>? CabinDetails { get; set; }
    }
}
