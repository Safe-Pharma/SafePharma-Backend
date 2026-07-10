namespace SafePharma.DAL
{
    public class PurchaseOrderItem
    {
        public Guid Id { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid PharmacyMedicineId { get; set; } 
        public PharmacyMedicine PharmacyMedicine { get; set; } = null!;
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
    }
}
