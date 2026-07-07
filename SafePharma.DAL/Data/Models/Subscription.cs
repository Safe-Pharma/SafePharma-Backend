namespace SafePharma.DAL
{
    public class Subscription : IAuditableEntity
    {
        public Guid Id { get; set; }
        public int SequenceNumber { get; set; }          // NEW — DB identity column
        public string PlanTier { get; set; }
        public string BillingCycle { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public Pharmacy Pharmacy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<PaymentVerification> PaymentVerifications { get; set; } = new List<PaymentVerification>();

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string ReferenceCode => $"SP-{CreatedAt.Year}-{SequenceNumber:D6}";
    }

    public enum SubscriptionStatus
    {
        AwaitingPayment,  //admin approved, payment email sent
        Active,           // payment confirmed, account can log in
        Cancelled
    }
}