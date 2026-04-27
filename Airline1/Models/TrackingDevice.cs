using System.ComponentModel.DataAnnotations;

namespace Airline1.Models
{
    // Hardware registry for NEO-M8N IoT devices
    public class TrackingDevice
    {
        [Key]
        [MaxLength(450)]
        public string DeviceId { get; set; } = string.Empty; // The Arduino Serial Number

        [Required]
        [MaxLength(255)]
        public string MqttTopic { get; set; } = string.Empty; // e.g., jose/betonio/loc

        public string? DeviceName { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastPingAt { get; set; }
    }
}
