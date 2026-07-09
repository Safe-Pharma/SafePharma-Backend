namespace SafePharma.BLL
{
    public class PurchaseOrderReadDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string SupplierName { get; set; } = null!;
        public int Lines { get; set; }
        public List<PurchaseOrderItemReadDto> Items { get; set; } = new List<PurchaseOrderItemReadDto>();

    }
}
