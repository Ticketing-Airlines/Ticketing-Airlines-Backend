using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Airline1.Models
{
    // Represents a single add-on purchased for a specific passenger
    public class BookingAddOn
    {
        [Key]
        public int BookingAddOnId { get; set; }

        public required Guid PassengerId { get; set; } // Links to BookingPassenger

        // The specific price rule that was active at the time of booking
        public required int AddOnPriceId { get; set; }

        // The price charged at the time of booking (denormalization for historical accuracy)
        [Column(TypeName = "decimal(18, 2)")]
        public required decimal PriceAtBooking { get; set; }

        // --- Navigation Properties ---
        [ForeignKey(nameof(PassengerId))]
        public BookingPassenger? Passenger { get; set; }

        [ForeignKey(nameof(AddOnPriceId))]
        public AddOnPrice? AddOnPrice { get; set; }
    }
}