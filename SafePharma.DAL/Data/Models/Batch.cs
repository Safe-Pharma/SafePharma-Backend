

using System.ComponentModel.DataAnnotations.Schema;

namespace SafePharma.DAL
{
    public class Batch : IAuditableEntity
    {
        [key]
        public Guid Id { get; set; }

        [ForeignKey("Medicine")]
        public Guid MedicineId { get; set; }
        public  Medicine Medicine { get; set; }

        //[ForeignKey("PurchaseOrderItem")]
        //public Guid PurchaseOrderItemId;
        //public required PurchaseOrderItem PurchaseOrderItem { get; set; }

        public int BatchNumber { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int QuantityReceived { get; set; }

        public int QuantityRemaining { get; set; }

        public decimal SellingPrice { get; set; }
        public decimal PurchasePrice { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
