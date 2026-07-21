namespace SafePharma.BLL
{
    public class CreateCustomerMedicineHistoryDto
    {
        public Guid? MedicineId { get; set; }
        public string? TradeName { get; set; }
        public string? ScientificName { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}