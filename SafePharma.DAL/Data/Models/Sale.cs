namespace SafePharma.DAL
{
    public class Sale
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public SalePaymentMethod PaymentMethod { get; set; } = SalePaymentMethod.Cash;
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaidByCash { get; set; }
        public decimal AmountPaidByCard { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; } = 0;

        public SaleStatus Status { get; set; } = SaleStatus.Open;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; } = new HashSet<SaleItem>();
    }
    public enum SaleStatus
    {
        Open,
        Cancelled,
        Completed
    }
    public enum SalePaymentMethod
    {
        Cash,
        Card,
        Mixed
    }
}