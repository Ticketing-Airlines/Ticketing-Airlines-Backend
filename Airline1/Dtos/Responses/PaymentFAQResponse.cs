namespace Airline1.Dtos.Responses
{
    public class PaymentFAQResponse
    {
        public int Id { get; set; }
        public required string Category { get; set; }
        public required string Question { get; set; }
        public required string Answer { get; set; }
        public required string Icon { get; set; }
        public required string Color { get; set; }
        public int Order { get; set; }
    }
}
