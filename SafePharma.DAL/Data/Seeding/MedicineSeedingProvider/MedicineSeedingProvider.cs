namespace SafePharma.DAL
{
    public static class MedicineSeedingProvider
    {
        public static List<Medicine> GetMedicines(List<Tax> taxes)
        {
            var seededAt = DateTime.UtcNow;

            var pharmacyIds = new[]
            {
                Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Guid.Parse("30000000-0000-0000-0000-000000000003"),
            };

            return pharmacyIds
                .SelectMany(pharmacyId => MedicinesFor(pharmacyId, taxes, seededAt))
                .ToList();
        }

        private static List<Medicine> MedicinesFor(Guid pharmacyId, List<Tax> taxes, DateTime seededAt)
        {
            var pharmacyTaxes = taxes.Where(t => t.PharmacyId == pharmacyId).ToList();

            var standardVat = pharmacyTaxes.First(t => t.Name == "Standard VAT");
            var zeroRated = pharmacyTaxes.First(t => t.Name == "Zero-Rated");
            var exempt = pharmacyTaxes.First(t => t.Name == "Exempt");
            var luxuryTax = pharmacyTaxes.First(t => t.Name == "Luxury Tax");

            return new List<Medicine>
            {
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    TradeNameAr = "بانادول",
                    TradeNameEn = "Panadol",
                    ScientificName = "Paracetamol",
                    Category = "Analgesic",
                    UnitOfSale = "Box",
                    UnitsPerPackage = 20,
                    PurchasePrice = 8.50m,
                    SellingPrice = 12.00m,
                    TaxId = standardVat.Id,
                    MinStockLevel = 50,
                    IsPrescriptionRequired = false,
                    IsControlled = false,
                    Manufacturer = "GSK",
                    CountryOfOrigin = "UK",
                    StorageConditions = "Store below 25°C",
                    TherapeuticCategory = "Pain Relief",
                    IsActive = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    TradeNameAr = "أوجمنتين",
                    TradeNameEn = "Augmentin",
                    ScientificName = "Amoxicillin/Clavulanic Acid",
                    Category = "Antibiotic",
                    UnitOfSale = "Box",
                    UnitsPerPackage = 14,
                    PurchasePrice = 35.00m,
                    SellingPrice = 48.00m,
                    TaxId = standardVat.Id,
                    MinStockLevel = 20,
                    IsPrescriptionRequired = true,
                    IsControlled = false,
                    Manufacturer = "GSK",
                    CountryOfOrigin = "Egypt",
                    StorageConditions = "Store below 30°C, protect from light",
                    TherapeuticCategory = "Antibiotics",
                    IsActive = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    TradeNameAr = "إنسولين لانتوس",
                    TradeNameEn = "Lantus Insulin",
                    ScientificName = "Insulin Glargine",
                    Category = "Hormone",
                    UnitOfSale = "Vial",
                    UnitsPerPackage = 1,
                    PurchasePrice = 120.00m,
                    SellingPrice = 150.00m,
                    TaxId = zeroRated.Id,
                    MinStockLevel = 10,
                    IsPrescriptionRequired = true,
                    IsControlled = true,
                    Manufacturer = "Sanofi",
                    CountryOfOrigin = "France",
                    StorageConditions = "Refrigerate 2-8°C",
                    TherapeuticCategory = "Diabetes",
                    IsActive = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    TradeNameAr = "أوراسيتام",
                    TradeNameEn = "Nootropil",
                    ScientificName = "Piracetam",
                    Category = "Nootropic",
                    UnitOfSale = "Box",
                    UnitsPerPackage = 30,
                    PurchasePrice = 45.00m,
                    SellingPrice = 60.00m,
                    TaxId = exempt.Id,
                    MinStockLevel = 15,
                    IsPrescriptionRequired = true,
                    IsControlled = false,
                    Manufacturer = "UCB",
                    CountryOfOrigin = "Belgium",
                    StorageConditions = "Store below 25°C",
                    TherapeuticCategory = "Neurology",
                    IsActive = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Medicine
                {
                    Id = Guid.NewGuid(),
                    TradeNameAr = "فيتامين سي بلس كولاجين",
                    TradeNameEn = "Vitamin C Plus Collagen",
                    ScientificName = "Ascorbic Acid + Collagen",
                    Category = "Cosmetic Supplement",
                    UnitOfSale = "Box",
                    UnitsPerPackage = 10,
                    PurchasePrice = 90.00m,
                    SellingPrice = 140.00m,
                    TaxId = luxuryTax.Id,
                    MinStockLevel = 25,
                    IsPrescriptionRequired = false,
                    IsControlled = false,
                    Manufacturer = "Nature's Bounty",
                    CountryOfOrigin = "USA",
                    StorageConditions = "Store below 25°C",
                    TherapeuticCategory = "Skin & Beauty",
                    IsActive = true,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
            };
        }
    }
}