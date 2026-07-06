namespace SafePharma.DAL
{
    public static class SupplierSeeding
    {
        public static List<Supplier> GetSuppliers()
        {
            var pharmacyIds = new[]
            {
                Guid.Parse("30000000-0000-0000-0000-000000000001"),
                Guid.Parse("30000000-0000-0000-0000-000000000002"),
                Guid.Parse("30000000-0000-0000-0000-000000000003"),
            };

            // same Country ids used in CountrySeeding
            var uae = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var ksa = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var egypt = Guid.Parse("10000000-0000-0000-0000-000000000003");
            var jordan = Guid.Parse("10000000-0000-0000-0000-000000000004");

            var seededAt = DateTime.UtcNow;

            return pharmacyIds
                .SelectMany(pharmacyId => SuppliersFor(pharmacyId, uae, ksa, egypt, jordan, seededAt))
                .ToList();
        }

        private static List<Supplier> SuppliersFor(
            Guid pharmacyId, Guid uae, Guid ksa, Guid egypt, Guid jordan, DateTime seededAt)
        {
            return new List<Supplier>
            {
                new Supplier
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "MedSupply Co.",
                    ContactPerson = "Ahmed Najjar",
                    Phone = "+971 4 555 1000",
                    Email = "sales@medsupply.ae",
                    TaxNumber = "100-234-556",
                    Address = "Sheikh Zayed Road, Dubai",
                    CountryId = uae,
                    Status = SupplierStatus.Active,
                    Outstanding = 12400m,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Supplier
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "GulfPharma",
                    ContactPerson = "Sarah Habib",
                    Phone = "+966 11 555 2200",
                    Email = "orders@gulfpharma.sa",
                    TaxNumber = "300-998-771",
                    Address = "King Fahd Road, Riyadh",
                    CountryId = ksa,
                    Status = SupplierStatus.Active,
                    Outstanding = 4820m,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Supplier
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "BioGen Distrib.",
                    ContactPerson = "Omar Sami",
                    Phone = "+20 2 555 3300",
                    Email = "info@biogen.eg",
                    TaxNumber = "450-112-004",
                    Address = "Nasr City, Cairo",
                    CountryId = egypt,
                    Status = SupplierStatus.Active,
                    Outstanding = 0m,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Supplier
                {
                    Id = Guid.NewGuid(),
                    PharmacyId = pharmacyId,
                    Name = "CarePlus",
                    ContactPerson = "Layla Karim",
                    Phone = "+962 6 555 4400",
                    Email = "hello@careplus.jo",
                    TaxNumber = "112-889-330",
                    Address = "Abdali, Amman",
                    CountryId = jordan,
                    Status = SupplierStatus.Inactive,
                    Outstanding = 0m,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
            };
        }
    }
}
