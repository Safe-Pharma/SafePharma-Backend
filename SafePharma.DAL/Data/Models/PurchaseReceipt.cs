namespace SafePharma.DAL
{
    public class PurchaseReceipt
    {
        public Guid Id {  get; set; }
        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        public Guid ReceivedBy { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceTotal { get; set; }
        public DateTime ReceivedAt { get; set; }
        public ICollection<PurchaseReceiptItem> Items { get; set; }
        = new HashSet<PurchaseReceiptItem>();
    }
}
