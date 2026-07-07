namespace SafePharma.BLL
{
    public class PaymentInstructionsDto
    {
        public Guid SubscriptionId { get; set; }
        public string ReferenceCode { get; set; }
        public string PlanTier { get; set; }
        public string BillingCycle { get; set; }
        public decimal AmountDue { get; set; }
        public string Currency { get; set; }
        public List<PaymentMethodReadDto> PaymentMethods { get; set; } = new();
    }
}