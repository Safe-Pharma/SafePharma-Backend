namespace SafePharma.BLL
{
    public class PurchaseOrderItemReadDto
    {
        public string MedicineName { get; set; } = null!;
        public int QuantityOrdered { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
