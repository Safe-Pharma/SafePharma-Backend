namespace SafePharma.BLL
{
    public class SupplierPaymentDto
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
