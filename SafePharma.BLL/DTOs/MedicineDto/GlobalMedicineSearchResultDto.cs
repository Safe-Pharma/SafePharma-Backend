namespace SafePharma.BLL
{
    public class GlobalMedicineSearchResultDto
    {
        public Guid Id { get; set; }
        public string TradeNameAr { get; set; } = string.Empty;
        public string TradeNameEn { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string UnitOfSale { get; set; } = string.Empty;
        public int UnitsPerPackage { get; set; }
        public string? Manufacturer { get; set; }
        public bool IsAlreadyInPharmacy { get; set; }
        public string DosageForm { get; set; } = string.Empty;
        public List<string> ManufacturerBarcodes { get; set; } = new();
        public string Strength { get; set; } = string.Empty;
    }
}