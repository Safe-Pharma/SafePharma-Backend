namespace SafePharma.DAL
{
    public static class CustomerPharmacyBalanceSeeding
    {
        public static List<CustomerPharmacyBalance> GetBalances(List<Customer> customers)
        {
            var seededAt = DateTime.UtcNow;

            var mediRx = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var alShifa = Guid.Parse("30000000-0000-0000-0000-000000000002");
            var nourAlHayat = Guid.Parse("30000000-0000-0000-0000-000000000003");

            var ahmed = customers[0];
            var sara = customers[1];
            var omar = customers[2];

            return new List<CustomerPharmacyBalance>
            {
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = ahmed.Id,
                    PharmacyId = mediRx,
                    TotalPaid = 300.00m,
                    LastPaymentAt = seededAt,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = ahmed.Id,
                    PharmacyId = alShifa,
                    TotalPaid = 150.00m,
                    LastPaymentAt = seededAt,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = sara.Id,
                    PharmacyId = mediRx,
                    TotalPaid = 120.50m,
                    LastPaymentAt = seededAt,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = omar.Id,
                    PharmacyId = nourAlHayat,
                    TotalPaid = 0m,
                    LastPaymentAt = null,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
            };
        }
    }
}