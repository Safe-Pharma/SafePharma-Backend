namespace SafePharma.BLL
{
    public class SubscriptionReadDto
    {
        public Guid Id { get; set; }
        public string PlanTier { get; set; }
        public string BillingCycle { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string PrimaryContactEmail { get; set; }
    }
}