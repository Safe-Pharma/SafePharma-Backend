namespace SafePharma.BLL
{
    public class RecordSupplierPaymentDto
    {
        public Guid SupplierId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
