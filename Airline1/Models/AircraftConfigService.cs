using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    public class AircraftConfiguration
    {
        [Key]
        [MaxLength(50)]
        public required string ConfigurationID { get; set; }

        // 💡 SeatingCapacity MOVED HERE
        public int TotalSeats { get; set; }

        [MaxLength(100)]
        public required string AircraftModel { get; set; }

        public ICollection<CabinConfigurationDetail> CabinDetails { get; set; } = [];
    }

    public class CabinConfigurationDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(AircraftConfiguration))]
        public required string ConfigurationID { get; set; }

        public required string CabinName { get; set; } // e.g., "Business Class"

        public int StartRow { get; set; }

        public int EndRow { get; set; }

        public string SeatMapLayout { get; set; } = "3-3"; 

    }
}