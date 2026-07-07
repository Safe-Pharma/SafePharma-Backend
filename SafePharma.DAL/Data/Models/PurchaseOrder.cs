namespace SafePharma.DAL
{
    public class PurchaseOrder
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; } = null!;
        public DateTime? ExpectedDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;
        public ICollection<PurchaseOrderItem> Items { get; set; } = new HashSet<PurchaseOrderItem>();
    }
}
