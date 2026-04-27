namespace Airline1.Dtos.Responses
{
    public class DeviceLocationResponse
    {
        public string DeviceId { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
