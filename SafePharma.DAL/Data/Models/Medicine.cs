namespace SafePharma.DAL
{
    public class Medicine : IAuditableEntity
    {
        public Guid Id { get; set; }

        public string TradeNameAr { get; set; } = string.Empty;
        public string TradeNameEn { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public string UnitOfSale { get; set; } = string.Empty;
        public int UnitsPerPackage { get; set; }

        public int MinStockLevel { get; set; }

        public bool IsPrescriptionRequired { get; set; }
        public bool IsControlled { get; set; }

        public string? Manufacturer { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? StorageConditions { get; set; }
        public string? TherapeuticCategory { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<MedicinePrice> Prices { get; set; } = new List<MedicinePrice>();
    }
}