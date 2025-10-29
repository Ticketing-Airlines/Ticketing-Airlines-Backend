namespace Airline1.Dtos.Responses
{
    public class AircraftConfigurationResponse
    {
        public string ConfigurationID { get; set; } = string.Empty;
        public string AircraftModel { get; set; } = string.Empty;
        public int TotalSeats { get; set; }

        public List<CabinDetailResponse> CabinDetails { get; set; } = [];
    }

    public class CabinDetailResponse
    {
        public string CabinName { get; set; } = string.Empty;
        public int StartRow { get; set; }
        public int EndRow { get; set; }
        public string SeatMapLayout { get; set; } = string.Empty;
    }
}
