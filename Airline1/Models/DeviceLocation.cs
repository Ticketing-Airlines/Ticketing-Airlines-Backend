using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    // History of GPS "pings" received from a tracked device
    public class DeviceLocation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(450)]
        public string DeviceId { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        public TrackingDevice? Device { get; set; }
    }
}
