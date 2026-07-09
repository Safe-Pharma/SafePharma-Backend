using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class MedicineMapper
    {
        public static MedicineDto ToDto(this PharmacyMedicine price)
        {
            var m = price.Medicine;
            return new MedicineDto
            {
                Id = m.Id,
                TradeNameAr = m.TradeNameAr,
                TradeNameEn = m.TradeNameEn,
                ScientificName = m.ScientificName,
                Category = m.Category,
                UnitOfSale = m.UnitOfSale,
                UnitsPerPackage = m.UnitsPerPackage,
                PurchasePrice = price.PurchasePrice,
                SellingPrice = price.SellingPrice,
                TaxId = price.TaxId,
                TaxName = price.Tax?.Name ?? string.Empty,
                MinStockLevel = price.MinStockLevel,
                IsPrescriptionRequired = m.IsPrescriptionRequired,
                IsControlled = m.IsControlled,
                Manufacturer = m.Manufacturer,
                CountryOfOrigin = m.CountryOfOrigin,
                StorageConditions = m.StorageConditions,
                TherapeuticCategory = m.TherapeuticCategory,
                IsActive = m.IsActive,
                ChangedAt = price.ChangedAt,
                ChangedBy = price.ChangedBy,
            };
        }

        public static GlobalMedicineSearchResultDto ToSearchResultDto(this Medicine m, bool isAlreadyInPharmacy)
        {
            return new GlobalMedicineSearchResultDto
            {
                Id = m.Id,
                TradeNameAr = m.TradeNameAr,
                TradeNameEn = m.TradeNameEn,
                ScientificName = m.ScientificName,
                Category = m.Category,
                UnitOfSale = m.UnitOfSale,
                UnitsPerPackage = m.UnitsPerPackage,
                Manufacturer = m.Manufacturer,
                IsAlreadyInPharmacy = isAlreadyInPharmacy,
            };
        }

        public static Medicine ToMedicineEntity(this MedicineCreateDto dto)
        {
            return new Medicine
            {
                TradeNameAr = dto.TradeNameAr,
                TradeNameEn = dto.TradeNameEn,
                ScientificName = dto.ScientificName,
                Category = dto.Category,
                UnitOfSale = dto.UnitOfSale,
                UnitsPerPackage = dto.UnitsPerPackage,
                IsPrescriptionRequired = dto.IsPrescriptionRequired,
                IsControlled = dto.IsControlled,
                Manufacturer = dto.Manufacturer,
                CountryOfOrigin = dto.CountryOfOrigin,
                StorageConditions = dto.StorageConditions,
                TherapeuticCategory = dto.TherapeuticCategory,
                IsActive = dto.IsActive,
            };
        }

        public static void ApplyTo(this PharmacyMedicineUpdateDto dto, PharmacyMedicine price)
        {
            price.TaxId = dto.TaxId;
            price.PurchasePrice = dto.PurchasePrice;
            price.SellingPrice = dto.SellingPrice;
            price.MinStockLevel = dto.MinStockLevel;
        }

        public static void ApplyTo(this GlobalMedicineUpdateDto dto, Medicine entity)
        {
            entity.TradeNameAr = dto.TradeNameAr;
            entity.TradeNameEn = dto.TradeNameEn;
            entity.ScientificName = dto.ScientificName;
            entity.Category = dto.Category;
            entity.UnitOfSale = dto.UnitOfSale;
            entity.UnitsPerPackage = dto.UnitsPerPackage;
            entity.IsPrescriptionRequired = dto.IsPrescriptionRequired;
            entity.IsControlled = dto.IsControlled;
            entity.Manufacturer = dto.Manufacturer;
            entity.CountryOfOrigin = dto.CountryOfOrigin;
            entity.StorageConditions = dto.StorageConditions;
            entity.TherapeuticCategory = dto.TherapeuticCategory;
            entity.IsActive = dto.IsActive;
        }
    }
}