namespace SafePharma.DAL
{
    public class PaymentVerification : IAuditableEntity
    {
        public Guid Id { get; set; }

        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }

        public string PaymentMethod { get; set; }        
        public string TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string ReceiptUrl { get; set; }

        public PaymentVerificationStatus Status { get; set; }
        public string? RejectionReason { get; set; }

        public Guid? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public enum PaymentVerificationStatus
    {
        Pending,
        Approved,
        Rejected
    }
}