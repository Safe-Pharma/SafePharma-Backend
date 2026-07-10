namespace SafePharma.DAL
{
    public class PharmacyMedicine : IAuditableEntity
    {
        public Guid Id { get; set; }

        public Guid MedicineId { get; set; }
        public Medicine Medicine { get; set; } = null!;

        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;
        public string SKU { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }

        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<PharmacyBarcode> PharmacyBarcodes { get; set; } = new HashSet<PharmacyBarcode>();
        public virtual ICollection<PharmacyMedicineTax> PharmacyMedicineTaxes { get; set; } = new HashSet<PharmacyMedicineTax>();
    }
}