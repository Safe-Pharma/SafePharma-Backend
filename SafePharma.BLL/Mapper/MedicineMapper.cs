using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class MedicineMapper
    {
        private static List<TaxSummaryDto> ToTaxSummaries(this PharmacyMedicine price)
        {
            return price.PharmacyMedicineTaxes
                .Select(pmt => new TaxSummaryDto
                {
                    Id = pmt.TaxId,
                    Name = pmt.Tax?.Name ?? string.Empty,
                    Rate = pmt.Tax?.Rate ?? 0m
                })
                .ToList();
        }

        private static string ComputeStockStatus(int availableQuantity, int minStockLevel)
        {
            if (availableQuantity <= 0) return "Out";
            if (availableQuantity < minStockLevel) return "Low";
            return "InStock";
        }

        // PharmacyMedicine now carries its own descriptive fields (denormalized at
        // creation time, whether imported from the global catalog or added locally),
        // so this reads straight off `price` — no join to Medicine required.
        public static MedicineDto ToDto(this PharmacyMedicine price, int availableQuantity = 0, int batchCount = 0)
        {
            return new MedicineDto
            {
                Id = price.Id,
                PharmacyMedicineId = price.Id,
                GlobalMedicineId = price.MedicineId,
                TradeNameAr = price.TradeNameAr,
                TradeNameEn = price.TradeNameEn,
                ScientificName = price.ScientificName,
                Category = price.Category,
                UnitOfSale = price.UnitOfSale,
                UnitsPerPackage = price.UnitsPerPackage,
                DosageForm = price.DosageForm,
                Strength = price.Strength,
                SKU = price.SKU,
                PharmacyBarcodes = price.PharmacyBarcodes.Select(b => b.Barcode).ToList(),
                PurchasePrice = price.PurchasePrice,
                SellingPrice = price.SellingPrice,
                Taxes = price.ToTaxSummaries(),
                MinStockLevel = price.MinStockLevel,
                IsPrescriptionRequired = price.IsPrescriptionRequired,
                IsControlled = price.IsControlled,
                Manufacturer = price.Manufacturer,
                CountryOfOrigin = price.CountryOfOrigin,
                StorageConditions = price.StorageConditions,
                TherapeuticCategory = price.TherapeuticCategory,
                IsActive = price.IsActive,
                ChangedAt = price.ChangedAt,
                ChangedBy = price.ChangedBy,
                AvailableQuantity = availableQuantity,
                NumberOfBatches = batchCount,
                StockStatus = ComputeStockStatus(availableQuantity, price.MinStockLevel),
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
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                IsAlreadyInPharmacy = isAlreadyInPharmacy,
                ManufacturerBarcodes = m.ManufacturerBarcodes.Select(b => b.Barcode).ToList(),
            };
        }

        // STEP 3 (local, no global match): build a PharmacyMedicine directly from the
        // create DTO. MedicineId is left null by the caller — this is a pharmacy-only record.
        public static PharmacyMedicine ToPharmacyMedicineEntity(this MedicineCreateDto dto)
        {
            return new PharmacyMedicine
            {
                TradeNameAr = dto.TradeNameAr,
                TradeNameEn = dto.TradeNameEn,
                ScientificName = dto.ScientificName,
                Category = dto.Category,
                UnitOfSale = dto.UnitOfSale,
                UnitsPerPackage = dto.UnitsPerPackage,
                DosageForm = dto.DosageForm,
                Strength = dto.Strength,
                IsPrescriptionRequired = dto.IsPrescriptionRequired,
                IsControlled = dto.IsControlled,
                Manufacturer = dto.Manufacturer,
                CountryOfOrigin = dto.CountryOfOrigin,
                StorageConditions = dto.StorageConditions,
                TherapeuticCategory = dto.TherapeuticCategory,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                MinStockLevel = dto.MinStockLevel,
                IsActive = dto.IsActive,
            };
        }

        // STEP 2 (import): copy the global medicine's descriptive fields onto the new
        // PharmacyMedicine at link time. MedicineId/pricing/etc. are set by the caller.
        public static void CopyDescriptiveFieldsTo(this Medicine medicine, PharmacyMedicine price)
        {
            price.TradeNameAr = medicine.TradeNameAr;
            price.TradeNameEn = medicine.TradeNameEn;
            price.ScientificName = medicine.ScientificName;
            price.Category = medicine.Category;
            price.UnitOfSale = medicine.UnitOfSale;
            price.DosageForm = medicine.DosageForm;
            price.Strength = medicine.Strength;
            price.UnitsPerPackage = medicine.UnitsPerPackage;
            price.IsPrescriptionRequired = medicine.IsPrescriptionRequired;
            price.IsControlled = medicine.IsControlled;
            price.Manufacturer = medicine.Manufacturer;
            price.CountryOfOrigin = medicine.CountryOfOrigin;
            price.StorageConditions = medicine.StorageConditions;
            price.TherapeuticCategory = medicine.TherapeuticCategory;
        }

        public static void ApplyTo(this PharmacyMedicineUpdateDto dto, PharmacyMedicine price)
        {
            price.PurchasePrice = dto.PurchasePrice;
            price.SellingPrice = dto.SellingPrice;
            price.MinStockLevel = dto.MinStockLevel;

            // Descriptive fields only make sense to edit here for local records.
            // Linked records should be corrected via the global catalog instead.
            if (price.MedicineId is not null)
            {
                return;
            }

            if (dto.TradeNameAr is not null) price.TradeNameAr = dto.TradeNameAr;
            if (dto.TradeNameEn is not null) price.TradeNameEn = dto.TradeNameEn;
            if (dto.ScientificName is not null) price.ScientificName = dto.ScientificName;
            if (dto.Category is not null) price.Category = dto.Category;
            if (dto.UnitOfSale is not null) price.UnitOfSale = dto.UnitOfSale;
            if (dto.DosageForm is not null) price.DosageForm = dto.DosageForm;
            if (dto.Strength is not null) price.Strength = dto.Strength;
            if (dto.UnitsPerPackage.HasValue) price.UnitsPerPackage = dto.UnitsPerPackage.Value;
            if (dto.IsPrescriptionRequired.HasValue) price.IsPrescriptionRequired = dto.IsPrescriptionRequired.Value;
            if (dto.IsControlled.HasValue) price.IsControlled = dto.IsControlled.Value;
            if (dto.Manufacturer is not null) price.Manufacturer = dto.Manufacturer;
            if (dto.CountryOfOrigin is not null) price.CountryOfOrigin = dto.CountryOfOrigin;
            if (dto.StorageConditions is not null) price.StorageConditions = dto.StorageConditions;
            if (dto.TherapeuticCategory is not null) price.TherapeuticCategory = dto.TherapeuticCategory;
        }

        public static void ApplyTo(this GlobalMedicineUpdateDto dto, Medicine entity)
        {
            entity.TradeNameAr = dto.TradeNameAr;
            entity.TradeNameEn = dto.TradeNameEn;
            entity.ScientificName = dto.ScientificName;
            entity.Category = dto.Category;
            entity.UnitOfSale = dto.UnitOfSale;
            entity.UnitsPerPackage = dto.UnitsPerPackage;
            entity.DosageForm = dto.DosageForm;
            entity.Strength = dto.Strength;
            entity.IsPrescriptionRequired = dto.IsPrescriptionRequired;
            entity.IsControlled = dto.IsControlled;
            entity.Manufacturer = dto.Manufacturer;
            entity.CountryOfOrigin = dto.CountryOfOrigin;
            entity.StorageConditions = dto.StorageConditions;
            entity.TherapeuticCategory = dto.TherapeuticCategory;
            entity.IsActive = dto.IsActive;
        }

        public static InventorySummaryDto ToInventorySummary(this StockAggregate? aggregate, int minStockLevel)
        {
            var available = aggregate?.AvailableQuantity ?? 0;

            return new InventorySummaryDto
            {
                TotalStock = aggregate?.TotalStock ?? 0,
                AvailableQuantity = available,
                NumberOfBatches = aggregate?.BatchCount ?? 0,
                ExpiringSoon = aggregate?.ExpiringSoon ?? 0,
                StockStatus = available <= 0 ? "Out" : available < minStockLevel ? "Low" : "InStock",
            };
        }

        public static MedicineDetailsDto ToDetailsDto(this PharmacyMedicine price, StockAggregate? aggregate)
        {
            return new MedicineDetailsDto
            {
                Id = price.Id,
                GlobalMedicineId = price.MedicineId,
                IsLocal = price.MedicineId is null,

                TradeNameAr = price.TradeNameAr,
                TradeNameEn = price.TradeNameEn,
                ScientificName = price.ScientificName,
                Category = price.Category,
                Manufacturer = price.Manufacturer,
                CountryOfOrigin = price.CountryOfOrigin,
                TherapeuticCategory = price.TherapeuticCategory,
                StorageConditions = price.StorageConditions,
                UnitOfSale = price.UnitOfSale,
                UnitsPerPackage = price.UnitsPerPackage,
                IsPrescriptionRequired = price.IsPrescriptionRequired,
                IsControlled = price.IsControlled,
                DosageForm = price.DosageForm,
                Strength = price.Strength,
                IsGlobalActive = price.Medicine?.IsActive,

                PharmacyMedicineId = price.Id,
                SKU = price.SKU,
                PurchasePrice = price.PurchasePrice,
                SellingPrice = price.SellingPrice,
                Taxes = price.ToTaxSummaries(),
                MinStockLevel = price.MinStockLevel,
                IsPharmacyActive = price.IsActive,

                ManufacturerBarcodes = price.Medicine?.ManufacturerBarcodes.Select(b => b.Barcode).ToList() ?? new(),
                PharmacyBarcodes = price.PharmacyBarcodes.Select(b => b.Barcode).ToList(),

                Inventory = aggregate.ToInventorySummary(price.MinStockLevel),
            };
        }

        public static Medicine ToMedicineEntity(this GlobalMedicineCreateDto dto)
        {
            return new Medicine
            {
                TradeNameAr = dto.TradeNameAr,
                TradeNameEn = dto.TradeNameEn,
                ScientificName = dto.ScientificName,
                Category = dto.Category,
                UnitOfSale = dto.UnitOfSale,
                UnitsPerPackage = dto.UnitsPerPackage,
                DosageForm = dto.DosageForm,
                Strength = dto.Strength,
                IsPrescriptionRequired = dto.IsPrescriptionRequired,
                IsControlled = dto.IsControlled,
                Manufacturer = dto.Manufacturer,
                CountryOfOrigin = dto.CountryOfOrigin,
                StorageConditions = dto.StorageConditions,
                TherapeuticCategory = dto.TherapeuticCategory,
                IsActive = dto.IsActive,
            };
        }

        public static GlobalMedicineDto ToGlobalDto(this Medicine m)
        {
            return new GlobalMedicineDto
            {
                Id = m.Id,
                TradeNameAr = m.TradeNameAr,
                TradeNameEn = m.TradeNameEn,
                ScientificName = m.ScientificName,
                Category = m.Category,
                UnitOfSale = m.UnitOfSale,
                UnitsPerPackage = m.UnitsPerPackage,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                IsPrescriptionRequired = m.IsPrescriptionRequired,
                IsControlled = m.IsControlled,
                Manufacturer = m.Manufacturer,
                CountryOfOrigin = m.CountryOfOrigin,
                StorageConditions = m.StorageConditions,
                TherapeuticCategory = m.TherapeuticCategory,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
            };
        }
    }
}
