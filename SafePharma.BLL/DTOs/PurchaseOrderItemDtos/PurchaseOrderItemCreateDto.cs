namespace SafePharma.BLL
{
    public class PurchaseOrderItemCreateDto
    {
        public Guid Id { get; set; }
        public Guid PharmacyMedicineId { get; set; }
        public int QuantityOrdered { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
