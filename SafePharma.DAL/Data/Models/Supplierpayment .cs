namespace SafePharma.DAL
{
    public static class SupplierPaymentMethods
    {
        public const string BankTransfer = "Bank Transfer";
        public const string Cheque = "Cheque";
        public const string Cash = "Cash";
        public const string CreditCard = "Credit Card";
        public const string Other = "Other";

        public static readonly string[] All =
        {
            BankTransfer, Cheque, Cash, CreditCard, Other
        };
    }

    
    public class SupplierPayment 
    {
        public Guid Id { get; set; }

        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public Guid RecordedBy { get; set; }
        public ApplicationUser RecordedByUser { get; set; } = null!;

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}