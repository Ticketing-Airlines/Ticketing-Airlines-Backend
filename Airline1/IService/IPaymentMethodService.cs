using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface IPaymentMethodService
    {
        Task<ActivePaymentMethodsDto> GetActiveMethodsAsync(string? category, bool? featured);
        Task<List<PaymentFAQResponse>> GetFAQsAsync();
    }

    public class ActivePaymentMethodsDto
    {
        public required List<PaymentMethodResponse> PaymentMethods { get; set; }
        public required List<string> Categories { get; set; }
        public int Total { get; set; }
        public required ActivePaymentMethodsMetadata Metadata { get; set; }
    }

    public class ActivePaymentMethodsMetadata
    {
        public DateTime LastUpdated { get; set; }
        public required string Version { get; set; }
    }
}
