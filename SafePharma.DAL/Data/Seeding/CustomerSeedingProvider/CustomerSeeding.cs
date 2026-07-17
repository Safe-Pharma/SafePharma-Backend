namespace SafePharma.DAL
{
    public static class CustomerSeeding
    {
        // Fixed IDs so other seed files (e.g. CustomerMedicineHistory seeding) can
        // reference a specific customer reliably across runs.
        public static readonly Guid AhmedHassanId = Guid.Parse("65000000-0000-0000-0000-000000000001");
        public static readonly Guid SaraMohamedId = Guid.Parse("65000000-0000-0000-0000-000000000002");
        public static readonly Guid OmarKhaledId = Guid.Parse("65000000-0000-0000-0000-000000000003");
        public static readonly Guid ManalIbrahimId = Guid.Parse("65000000-0000-0000-0000-000000000004");

        public static List<Customer> GetCustomers()
        {
            var seededAt = DateTime.UtcNow;

            return new List<Customer>
            {
                new Customer
                {
                    Id = AhmedHassanId,
                    Name = "Ahmed Hassan",
                    Phone = "+201001234567",
                    Email = "ahmed.hassan@example.com",
                    Address = "12 El Nasr St, Ismailia",
                    DateOfBirth = new DateTime(1988, 3, 14),
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Customer
                {
                    Id = SaraMohamedId,
                    Name = "Sara Mohamed",
                    Phone = "+201009876543",
                    Email = "sara.mohamed@example.com",
                    Address = "5 Talaat Harb St, Cairo",
                    DateOfBirth = new DateTime(1995, 7, 22),
                    Status = CustomerStatus.Active,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Customer
                {
                    Id = OmarKhaledId,
                    Name = "Omar Khaled",
                    Phone = "+201112223344",
                    Email = null,
                    Address = "20 Corniche Road, Suez",
                    DateOfBirth = new DateTime(1972, 11, 2),
                    Notes = "Chronic hypertension patient",
                    Status = CustomerStatus.Active,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new Customer
                {
                    Id = ManalIbrahimId,
                    Name = "Manal Ibrahim",
                    Phone = "+201223334455",
                    Email = "manal.ibrahim@example.com",
                    Address = "8 Gomhoria St, Zagazig",
                    DateOfBirth = new DateTime(2001, 1, 30),
                    Status = CustomerStatus.Inactive,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
            };
        }
    }
}