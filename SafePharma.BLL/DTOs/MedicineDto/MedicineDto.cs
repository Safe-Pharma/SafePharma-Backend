namespace SafePharma.BLL
{
    public class MedicineDto
    {
        public Guid Id { get; set; }
        public Guid PharmacyMedicineId { get; set; }
        // Set when this record is linked to the global catalog; null for pharmacy-local medicines.
        public Guid? GlobalMedicineId { get; set; }
        public string TradeNameAr { get; set; } = string.Empty;
        public string TradeNameEn { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string UnitOfSale { get; set; } = string.Empty;
        public int UnitsPerPackage { get; set; }
        public string DosageForm { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public List<string> PharmacyBarcodes { get; set; } = new();
        public string SKU { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public List<TaxSummaryDto> Taxes { get; set; } = new();
        public int MinStockLevel { get; set; }
        public bool IsPrescriptionRequired { get; set; }
        public bool IsControlled { get; set; }
        public string? Manufacturer { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? StorageConditions { get; set; }
        public string? TherapeuticCategory { get; set; }
        public bool IsActive { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }

        // List-view stock snapshot — a cheap aggregate, not the full InventorySummaryDto.
        public int AvailableQuantity { get; set; }
        public int NumberOfBatches { get; set; }
        public string StockStatus { get; set; } = "InStock"; // "InStock" | "Low" | "Out"
    }
}