namespace SafePharma.DAL
{
    public class SubscriptionPlan : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Tier { get; set; }            // "Starter" | "Professional" | "Enterprise" — must match Subscription.PlanTier
        public string DisplayName { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public string FeaturesJson { get; set; } = "[]";   // JSON string array, e.g. ["5 branches","Unlimited users"]
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}