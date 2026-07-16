namespace SafePharma.BLL
{
    public class MedicineDetailsDto
    {
        // Global medicine info (only meaningful when GlobalMedicineId is set)
        public Guid Id { get; set; }
        public Guid? GlobalMedicineId { get; set; }
        public bool IsLocal { get; set; }
        public string TradeNameAr { get; set; } = string.Empty;
        public string TradeNameEn { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? TherapeuticCategory { get; set; }
        public string? StorageConditions { get; set; }
        public string UnitOfSale { get; set; } = string.Empty;
        public int UnitsPerPackage { get; set; }
        public bool IsPrescriptionRequired { get; set; }
        public bool IsControlled { get; set; }
        public string DosageForm { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public bool? IsGlobalActive { get; set; }

        // Pharmacy-specific info
        public Guid PharmacyMedicineId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public List<TaxSummaryDto> Taxes { get; set; } = new();
        public int MinStockLevel { get; set; }
        public bool IsPharmacyActive { get; set; }

        // Barcodes
        public List<string> ManufacturerBarcodes { get; set; } = new();
        public List<string> PharmacyBarcodes { get; set; } = new();

        // Inventory
        public InventorySummaryDto Inventory { get; set; } = new();
    }
}