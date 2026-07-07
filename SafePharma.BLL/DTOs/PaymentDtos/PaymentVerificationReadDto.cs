namespace SafePharma.BLL
{
    public class PaymentVerificationReadDto
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string PharmacyName { get; set; }
        public string PlanTier { get; set; }
        public string BillingCycle { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string ReceiptUrl { get; set; }
        public string Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}