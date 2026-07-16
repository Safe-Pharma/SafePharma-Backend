namespace SafePharma.DAL
{
    // One row per medicine a customer is/was taking. IsActive marks medicines the
    // customer is currently taking (e.g. ongoing/chronic treatment).
    //
    // Linked to the GLOBAL medicine catalog (not a specific pharmacy's PharmacyMedicine),
    // since the customer is a global entity too and this is a medical history, not a
    // sale record — the same medicine could have been bought from any pharmacy.
    public class CustomerMedicineHistory
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        // Set when the medicine exists in the global catalog.
        public Guid? MedicineId { get; set; }
        public Medicine? Medicine { get; set; }

        // Free-text fallback entered by the pharmacist when the medicine is NOT found
        // in the global catalog. Only meaningful when MedicineId is null.
        public string? ScientificName { get; set; }

        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }

        public bool IsActive { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
