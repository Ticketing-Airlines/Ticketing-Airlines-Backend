namespace Airline1.Dtos.Responses
{
    public class SeatSaleResponse
    {
        public string Id { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public string DestinationAirportCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string CountryIso2 { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; } = null!;

        public SeatSalePricingResponse Pricing { get; set; } = new();
        public SeatSaleValidityResponse Validity { get; set; } = new();
        public SeatSaleAvailabilityResponse Availability { get; set; } = new();

        public List<string> Features { get; set; } = new();
        public bool Featured { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SeatSalePricingResponse
    {
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int Discount { get; set; }
        public string Currency { get; set; } = null!;
        public string PriceNote { get; set; } = null!;
    }

    public class SeatSaleValidityResponse
    {
        public DateOnly TravelPeriodStart { get; set; }
        public DateOnly TravelPeriodEnd { get; set; }
        public DateOnly BookingDeadline { get; set; }
    }

    public class SeatSaleAvailabilityResponse
    {
        public int SeatsLeft { get; set; }
        public int TotalSeats { get; set; }
        public bool IsLowAvailability { get; set; }
    }
}
