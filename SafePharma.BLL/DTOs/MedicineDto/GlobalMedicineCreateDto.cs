namespace SafePharma.BLL
{
    // Pure global-catalog create — no price/tax/SKU/stock fields, since those
    // belong to a pharmacy's PharmacyMedicine record, not the global Medicine.
    public class GlobalMedicineCreateDto
    {
        public string TradeNameAr { get; set; } = string.Empty;
        public string TradeNameEn { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string UnitOfSale { get; set; } = string.Empty;
        public int UnitsPerPackage { get; set; }
        public bool IsPrescriptionRequired { get; set; }
        public bool IsControlled { get; set; }
        public string? Manufacturer { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? StorageConditions { get; set; }
        public string? TherapeuticCategory { get; set; }
        public bool IsActive { get; set; } = true;
        public string DosageForm { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
    }
}