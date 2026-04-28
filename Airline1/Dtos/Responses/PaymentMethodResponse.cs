namespace Airline1.Dtos.Responses
{
    public class PaymentMethodResponse
    {
        public required string Id { get; set; }
        public required string Category { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string ProcessingTime { get; set; }
        public required PaymentFeeDto Fee { get; set; }
        public required string Color { get; set; }
        public bool Featured { get; set; }
        public bool IsActive { get; set; }
        public required List<string> Providers { get; set; }
        public required string Icon { get; set; }
        public required List<string> Features { get; set; }
        public required PaymentAvailabilityDto Availability { get; set; }
        public int Order { get; set; }
    }

    public class PaymentFeeDto
    {
        public required string Type { get; set; }
        public decimal Amount { get; set; }
        public decimal? FixedAmount { get; set; }
        public required string Currency { get; set; }
        public required string DisplayText { get; set; }
    }

    public class PaymentAvailabilityDto
    {
        public bool IsAvailable { get; set; }
        public string? MaintenanceSchedule { get; set; }
    }
}
