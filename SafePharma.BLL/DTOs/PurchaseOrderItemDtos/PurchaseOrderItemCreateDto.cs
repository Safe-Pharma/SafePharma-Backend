namespace SafePharma.BLL
{
    public class PurchaseOrderItemCreateDto
    {
        public Guid MedicineId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
