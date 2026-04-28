namespace Airline1.Dtos.Responses
{
    public class SeatSaleConfigResponse
    {
        public DateTime SaleEndDate { get; set; }
        public string SaleTitle { get; set; } = null!;
        public string SaleSubtitle { get; set; } = null!;
        public bool IsActive { get; set; }
        public string HeroMessage { get; set; } = null!;
        public List<TermsConditionResponse> TermsAndConditions { get; set; } = new();
        public SeatSaleMetadataResponse Metadata { get; set; } = new();
    }

    public class TermsConditionResponse
    {
        public int Id { get; set; }
        public string Icon { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Color { get; set; } = null!;
    }

    public class SeatSaleMetadataResponse
    {
        public DateTime LastUpdated { get; set; }
        public string Version { get; set; } = null!;
    }
}
