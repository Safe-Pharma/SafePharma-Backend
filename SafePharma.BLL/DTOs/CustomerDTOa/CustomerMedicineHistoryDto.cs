namespace SafePharma.BLL
{
    public class CustomerMedicineHistoryDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        // Set when linked to the global catalog; null for a free-text entry.
        public Guid? MedicineId { get; set; }
        public bool IsGlobalMatch { get; set; }

        // TradeNameEn from the global Medicine when linked, otherwise the
        // pharmacist's free-text scientific name.
        public string MedicineName { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;

        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
