namespace SafePharma.BLL
{
    public class ReadSaleItemsDto
    {
        public Guid Id { get; set; }
        public Guid PharmacyMedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;

        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;

        public Guid BatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
