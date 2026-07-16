namespace SafePharma.BLL
{
    public class PharmacyMedicineUpdateDto
    {
        public List<Guid> TaxIds { get; set; } = new();
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int MinStockLevel { get; set; }
        public string? SKU { get; set; }

        // Descriptive fields — only applied when this PharmacyMedicine is local
        // (no GlobalMedicineId). For linked records these are ignored; edit the
        // global catalog entry instead.
        public string? TradeNameAr { get; set; }
        public string? TradeNameEn { get; set; }
        public string? ScientificName { get; set; }
        public string? Category { get; set; }
        public string? UnitOfSale { get; set; }
        public string? DosageForm { get; set; }
        public string? Strength { get; set; }
        public int? UnitsPerPackage { get; set; }
        public bool? IsPrescriptionRequired { get; set; }
        public bool? IsControlled { get; set; }
        public string? Manufacturer { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? StorageConditions { get; set; }
        public string? TherapeuticCategory { get; set; }
    }
}
