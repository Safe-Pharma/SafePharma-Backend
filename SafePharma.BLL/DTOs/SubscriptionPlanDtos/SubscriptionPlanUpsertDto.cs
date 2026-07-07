namespace SafePharma.BLL
{
    public class SubscriptionPlanUpsertDto
    {
        public string Tier { get; set; }
        public string DisplayName { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public List<string> Features { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
    }
}