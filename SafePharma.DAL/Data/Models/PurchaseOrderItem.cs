namespace SafePharma.DAL
{
    public class PurchaseOrderItem
    {
        public Guid Id { get; set; }

        public Guid PurchaseOrderId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal UnitPrice { get; set; }
        //public Guid MedicineId { get; set; } 
        //public Medicines Medicine { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
    }
}
