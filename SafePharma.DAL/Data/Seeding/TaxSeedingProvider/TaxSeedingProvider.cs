namespace SafePharma.DAL
{
    public static class TaxSeedingProvider
    {
        public static List<Tax> GetTaxes()
        {
            var pharmacyIds = new[]
            {
                Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Guid.Parse("30000000-0000-0000-0000-000000000003"),
            };

            var seededAt = DateTime.UtcNow;

            return pharmacyIds
                .SelectMany(pharmacyId => TaxesFor(pharmacyId, seededAt))
                .ToList();
        }

        private static List<Tax> TaxesFor(Guid pharmacyId, DateTime seededAt)
        {
            return new List<Tax>
            {
                new Tax
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "Standard VAT",
                    Rate = 5.00m,
                    Status = TaxStatus.Active,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Tax
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "Zero-Rated",
                    Rate = 0.00m,
                    Status = TaxStatus.Active,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Tax
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "Exempt",
                    Rate = 0.00m,
                    Status = TaxStatus.Active,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Tax
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "Luxury Tax",
                    Rate = 15.00m,
                    Status = TaxStatus.Inactive,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
            };
        }
    }
}