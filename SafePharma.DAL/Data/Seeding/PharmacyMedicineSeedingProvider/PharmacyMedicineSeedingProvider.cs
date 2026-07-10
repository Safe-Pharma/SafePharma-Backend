namespace SafePharma.DAL
{
    public static class PharmacyMedicineSeedingProvider
    {
        public static List<PharmacyMedicine> GetPharmacyMedicines(List<Medicine> medicines, List<Tax> taxes)
        {
            var seededAt = DateTime.UtcNow;

            var pharmacyIds = new[]
            {
                Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Guid.Parse("30000000-0000-0000-0000-000000000003"),
            };

            // TradeNameEn -> (PurchasePrice, SellingPrice, TaxName, MinStockLevel)
            var priceSheet = new Dictionary<string, (decimal Purchase, decimal Selling, string TaxName, int MinStockLevel)>
            {
                ["Panadol"] = (8.50m, 12.00m, "Standard VAT", 50),
                ["Augmentin"] = (35.00m, 48.00m, "Standard VAT", 20),
                ["Lantus Insulin"] = (120.00m, 150.00m, "Zero-Rated", 10),
                ["Nootropil"] = (45.00m, 60.00m, "Exempt", 15),
                ["Vitamin C Plus Collagen"] = (90.00m, 140.00m, "Luxury Tax", 25),
            };

            var prices = new List<PharmacyMedicine>();

            foreach (var pharmacyId in pharmacyIds)
            {
                var pharmacyTaxes = taxes.Where(t => t.PharmacyId == pharmacyId).ToList();

                foreach (var medicine in medicines)
                {
                    if (!priceSheet.TryGetValue(medicine.TradeNameEn, out var priceInfo))
                    {
                        continue;
                    }

                    var tax = pharmacyTaxes.First(t => t.Name == priceInfo.TaxName);

                    var pharmacyMedicine = new PharmacyMedicine
                    {
                        Id = Guid.NewGuid(),
                        MedicineId = medicine.Id,
                        PharmacyId = pharmacyId,
                        PurchasePrice = priceInfo.Purchase,
                        SellingPrice = priceInfo.Selling,
                        MinStockLevel = priceInfo.MinStockLevel,
                        ChangedAt = seededAt,
                        ChangedBy = "system",
                    };
                    pharmacyMedicine.PharmacyMedicineTaxes.Add(new PharmacyMedicineTax
                    {
                        PharmacyMedicineId = pharmacyMedicine.Id,
                        TaxId = tax.Id,
                    });

                    prices.Add(pharmacyMedicine);
                }
            }

            return prices;
        }
    }
}