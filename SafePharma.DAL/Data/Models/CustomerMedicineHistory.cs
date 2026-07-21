namespace SafePharma.DAL
{
    public class CustomerMedicineHistory
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid? MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public string? TradeName { get; set; }
        public string? ScientificName { get; set; }

        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }

        public bool IsActive { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}