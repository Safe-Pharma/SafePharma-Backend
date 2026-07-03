namespace SafePharma.DAL
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public string PlanTier { get; set; }        // "Starter" | "Professional" | "Enterprise"
        public string BillingCycle { get; set; }     // "monthly" | "yearly"
        public SubscriptionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public Pharmacy Pharmacy { get; set; }
    }

    public enum SubscriptionStatus
    {
        PendingReview,    // just submitted, waiting on admin
        AwaitingPayment,  // admin approved, payment email sent
        Active,           // payment confirmed, account can log in
        Rejected
    }
}