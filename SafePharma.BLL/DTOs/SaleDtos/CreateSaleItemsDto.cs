namespace SafePharma.BLL
{
    public class CreateSaleItemsDto
    {
        public Guid PharmacyMedicineId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BatchId { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
