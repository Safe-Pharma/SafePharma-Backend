namespace SafePharma.BLL
{
    public class CreateCustomerMedicineHistoryDto
    {
        // Provide MedicineId when the medicine exists in the global catalog.
        public Guid? MedicineId { get; set; }

        // Both required when MedicineId is null — the pharmacist's free-text entry
        // for a medicine that isn't in the global catalog.
        public string? TradeName { get; set; }
        public string? ScientificName { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public int Quantity { get; set; } = 1;
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
    }
}