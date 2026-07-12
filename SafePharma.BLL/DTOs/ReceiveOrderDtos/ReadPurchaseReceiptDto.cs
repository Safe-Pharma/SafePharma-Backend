namespace SafePharma.BLL
{
    public class ReadPurchaseReceiptDto
    {
        public Guid PurchaseOrderId { get; set; }
        public string? InvoiceNumber { get; set; } = null!;
        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public Guid ReceivedBy { get; set; }
        public DateTime ReceivedAt { get; set; }
        public List<ReadPurchaseReceiptItemDto> Items { get; set; } = new();

    }
}