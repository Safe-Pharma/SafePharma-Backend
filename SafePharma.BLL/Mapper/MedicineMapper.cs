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

        public static MedicineDto ToDto(this PharmacyMedicine price, int availableQuantity = 0, int batchCount = 0)
        {
            var m = price.Medicine;
            return new MedicineDto
            {
                Id = m.Id,
                PharmacyMedicineId = price.Id,
                TradeNameAr = m.TradeNameAr,
                TradeNameEn = m.TradeNameEn,
                ScientificName = m.ScientificName,
                Category = m.Category,
                UnitOfSale = m.UnitOfSale,
                UnitsPerPackage = m.UnitsPerPackage,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                SKU = price.SKU,
                PharmacyBarcodes = price.PharmacyBarcodes.Select(b => b.Barcode).ToList(),
                PurchasePrice = price.PurchasePrice,
                SellingPrice = price.SellingPrice,
                Taxes = price.ToTaxSummaries(),
                MinStockLevel = price.MinStockLevel,
                IsPrescriptionRequired = m.IsPrescriptionRequired,
                IsControlled = m.IsControlled,
                Manufacturer = m.Manufacturer,
                CountryOfOrigin = m.CountryOfOrigin,
                StorageConditions = m.StorageConditions,
                TherapeuticCategory = m.TherapeuticCategory,
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

        public static void ApplyTo(this PharmacyMedicineUpdateDto dto, PharmacyMedicine price)
        {
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
            var m = price.Medicine;
            return new MedicineDetailsDto
            {
                Id = m.Id,
                TradeNameAr = m.TradeNameAr,
                TradeNameEn = m.TradeNameEn,
                ScientificName = m.ScientificName,
                Category = m.Category,
                Manufacturer = m.Manufacturer,
                CountryOfOrigin = m.CountryOfOrigin,
                TherapeuticCategory = m.TherapeuticCategory,
                StorageConditions = m.StorageConditions,
                UnitOfSale = m.UnitOfSale,
                UnitsPerPackage = m.UnitsPerPackage,
                IsPrescriptionRequired = m.IsPrescriptionRequired,
                IsControlled = m.IsControlled,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                IsGlobalActive = m.IsActive,

                PharmacyMedicineId = price.Id,
                SKU = price.SKU,
                PurchasePrice = price.PurchasePrice,
                SellingPrice = price.SellingPrice,
                Taxes = price.ToTaxSummaries(),
                MinStockLevel = price.MinStockLevel,
                IsPharmacyActive = price.IsActive,

                ManufacturerBarcodes = m.ManufacturerBarcodes.Select(b => b.Barcode).ToList(),
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