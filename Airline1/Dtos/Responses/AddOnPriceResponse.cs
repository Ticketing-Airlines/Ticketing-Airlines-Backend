namespace Airline1.Dtos.Responses
{
    public class AddOnPriceResponse
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int AddOnId { get; set; }
        public decimal PriceAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}