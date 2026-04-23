using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class ConfirmPaymentRequest
    {
        [Required]
        [MaxLength(50)]
        public required string PaymentMethod { get; set; } // GCash, PayMaya, CreditCard

        // Optional: simulate failure for testing
        public bool SimulateFailure { get; set; } = false;
    }
}
