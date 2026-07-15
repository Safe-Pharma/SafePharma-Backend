namespace SafePharma.DAL
{
    public class SaleItem
    {
        public Guid Id { get; set; }
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; } = null!;
        public Guid PharmacyMedicineId { get; set; }
        public PharmacyMedicine PharmacyMedicine { get; set; } = null!;
        public Guid CustomerId { get; set; }
        //public Customer customer { get; set; } = null!;
        public Guid BatchId { get; set; }
        public Batch Batch { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }

    }
}
