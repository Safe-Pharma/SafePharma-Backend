namespace SafePharma.DAL
{
    public static class CustomerSeeding
    {
        public static List<Customer> GetCustomers()
        {
            var seededAt = DateTime.UtcNow;

            return new List<Customer>
            {
                new Customer
                {
                    Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
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