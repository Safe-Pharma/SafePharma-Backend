using SafePharma.DAL.Data.Models;

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
        public string DosageForm { get; set; } = string.Empty;

        public string Strength { get; set; } = string.Empty;
        public int UnitsPerPackage { get; set; }

        public bool IsPrescriptionRequired { get; set; }
        public bool IsControlled { get; set; }

        public string? Manufacturer { get; set; }
        public string? CountryOfOrigin { get; set; }
        public string? StorageConditions { get; set; }
        public string? TherapeuticCategory { get; set; }

        public bool IsGlobal { get; set; } = true;
        public Guid? OwnerPharmacyId { get; set; }
        public Pharmacy? OwnerPharmacy { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PharmacyMedicine> PharmacyMedicines { get; set; } = new List<PharmacyMedicine>();

        public virtual ICollection<ManufacturerBarcode> ManufacturerBarcodes { get; set; } = new HashSet<ManufacturerBarcode>();

    }
}