namespace SafePharma.DAL
{
    public class PurchaseReceiptItem
    {
        public Guid Id { get; set; }
        public Guid PurchaseReceiptId { get; set; }
        public PurchaseReceipt PurchaseReceipt { get; set; } = null!;
        public Guid PharmacyMedicineId { get; set; }
        public PharmacyMedicine PharmacyMedicine { get; set; } = null!;
        public string MedicineName { get; set; } = null;
        public Guid PurchaseOrderItemId { get; set; }
        public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;
        public int Quantity { get; set; }
        public string BatchNumber { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
