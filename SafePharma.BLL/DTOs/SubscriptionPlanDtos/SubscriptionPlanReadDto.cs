namespace SafePharma.BLL
{
    public class SubscriptionPlanReadDto
    {
        public Guid Id { get; set; }
        public string Tier { get; set; }
        public string DisplayName { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public string Currency { get; set; }
        public List<string> Features { get; set; } = new();
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
    }
}