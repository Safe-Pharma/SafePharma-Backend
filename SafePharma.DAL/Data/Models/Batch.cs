

using System.ComponentModel.DataAnnotations.Schema;

namespace SafePharma.DAL
{
    public class Batch : IAuditableEntity
    {
        [key]
        public Guid Id { get; set; }

        [ForeignKey("Medicine")]
        public Guid MedicineId { get; set; }
        public  PharmacyMedicine Medicine { get; set; }

        [ForeignKey("PurchaseReceiptItem")]
        public Guid PurchaseReceiptItemId;
        public PurchaseReceiptItem PurchaseReceiptItem { get; set; }

        [ForeignKey("Pharmacy")]
        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;

        public string BatchNumber { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public int QuantityReceived { get; set; }

        public int QuantityRemaining { get; set; }

        public decimal SellingPrice { get; set; }
        public decimal PurchasePrice { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
