using SafePharma.DAL;

namespace SafePharma.BLL
{
    public class ReadPurchaseReceiptItemDto
    {
        public Guid PurchaseReceiptItemId { get; set; }
        public Guid PurchaseOrderItemId { get; set; }

        public Guid PharmacyMedicineId { get; set; }

        public string MedicineName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string BatchNumber { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal SellingPrice { get; set; }
    }
}
