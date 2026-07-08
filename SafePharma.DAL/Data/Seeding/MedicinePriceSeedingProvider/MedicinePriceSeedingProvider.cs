namespace SafePharma.DAL
{
    public static class MedicinePriceSeedingProvider
    {
        public static List<MedicinePrice> GetMedicinePrices(List<Medicine> medicines, List<Tax> taxes)
        {
            var seededAt = DateTime.UtcNow;

            var pharmacyIds = new[]
            {
                Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Guid.Parse("30000000-0000-0000-0000-000000000003"),
            };

            // TradeNameEn -> (PurchasePrice, SellingPrice, TaxName)
            var priceSheet = new Dictionary<string, (decimal Purchase, decimal Selling, string TaxName)>
            {
                ["Panadol"] = (8.50m, 12.00m, "Standard VAT"),
                ["Augmentin"] = (35.00m, 48.00m, "Standard VAT"),
                ["Lantus Insulin"] = (120.00m, 150.00m, "Zero-Rated"),
                ["Nootropil"] = (45.00m, 60.00m, "Exempt"),
                ["Vitamin C Plus Collagen"] = (90.00m, 140.00m, "Luxury Tax"),
            };

            var prices = new List<MedicinePrice>();

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

                    prices.Add(new MedicinePrice
                    {
                        Id = Guid.NewGuid(),
                        MedicineId = medicine.Id,
                        PharmacyId = pharmacyId,
                        TaxId = tax.Id,
                        PurchasePrice = priceInfo.Purchase,
                        SellingPrice = priceInfo.Selling,
                        ChangedAt = seededAt,
                        ChangedBy = "system",
                    });
                }
            }

            return prices;
        }
    }
}