namespace Airline1.Dtos.Requests
{
    public class CreateAircraftConfigurationRequest
    {
        public required string ConfigurationID { get; set; }
        public required string AircraftModel { get; set; }
        public int TotalSeats { get; set; }

        public List<CabinDetailDto> CabinDetails { get; set; } = [];
    }

    public class CabinDetailDto
    {
        public required string CabinName { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public string SeatMapLayout { get; set; } = "3-3";
    }
}
