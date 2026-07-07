namespace SafePharma.BLL
{
    public class PurchaseOrderCreateDto
    {
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public Guid SupplierId { get; set; }
        public List<PurchaseOrderItemCreateDto> Items { get; set; } = new();

    }
}
