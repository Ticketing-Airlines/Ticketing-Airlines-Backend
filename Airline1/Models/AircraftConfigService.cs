// This will NOT be in the Airline1.Models namespace
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class AircraftConfiguration
    {
        // This ID is the primary key that your Airline1.Models.Aircraft references.
        [Key]
        [MaxLength(50)]
        public required string ConfigurationID { get; set; }

        // 💡 SeatingCapacity MOVED HERE
        public int TotalSeats { get; set; }

        [MaxLength(100)]
        public required string AircraftModel { get; set; }

        // Navigation property for the detailed row/seat layout (e.g., Row 1-4 is Business)
        public ICollection<CabinConfigurationDetail> CabinDetails { get; set; } = [];
    }

    // Example detail model to define the row structure
    public class CabinConfigurationDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(AircraftConfiguration))]
        public required string ConfigurationID { get; set; }

        public required string CabinName { get; set; } // e.g., "Business Class"

        public int StartRow { get; set; }

        public int EndRow { get; set; }

        public string SeatMapLayout { get; set; } = "3-3"; // e.g., "2-2", "3-3"

        // This model is the true blueprint for the seat map generation!
    }
}