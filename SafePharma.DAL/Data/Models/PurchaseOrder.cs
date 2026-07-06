namespace SafePharma.DAL
{
    public class PurchaseOrder
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        //public Guid SupplierId { get; set; }
        //public Suppliers Supplier { get; set; } = null!;
        public ICollection<PurchaseOrderItem> PurchaseOrdersItems { get; set; } = new HashSet<PurchaseOrderItem>();
    }
}
