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

        public List<PurchaseReceiptItemReadDto> Items { get; set; } = new();
    }

    public class PurchaseReceiptItemReadDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public Guid PharmacyMedicineId { get; set; }
        public string MedicineName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}