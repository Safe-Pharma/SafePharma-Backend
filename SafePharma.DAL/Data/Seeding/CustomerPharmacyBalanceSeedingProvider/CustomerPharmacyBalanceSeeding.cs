namespace SafePharma.DAL
{
    public static class CustomerPharmacyBalanceSeeding
    {
        public static List<CustomerPharmacyBalance> GetBalances()
        {
            var seededAt = DateTime.UtcNow;

            var mediRx = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var alShifa = Guid.Parse("30000000-0000-0000-0000-000000000002");
            var nourAlHayat = Guid.Parse("30000000-0000-0000-0000-000000000003");

            // A customer's payments are split across whichever pharmacies they've
            // actually visited — never a single total, since Customer is global.
            return new List<CustomerPharmacyBalance>
            {
                // Ahmed Hassan paid at two different pharmacies.
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerSeeding.AhmedHassanId,
                    PharmacyId = mediRx,
                    TotalPaid = 300.00m,
                    LastPaymentAt = seededAt,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerSeeding.AhmedHassanId,
                    PharmacyId = alShifa,
                    TotalPaid = 150.00m,
                    LastPaymentAt = seededAt,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },

                // Sara Mohamed only ever visited MediRx.
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerSeeding.SaraMohamedId,
                    PharmacyId = mediRx,
                    TotalPaid = 120.50m,
                    LastPaymentAt = seededAt,
                    CreatedAt = seededAt,
                    UpdatedAt = seededAt,
                },

                // Omar Khaled visited Nour Al-Hayat but hasn't paid anything there yet.
                new CustomerPharmacyBalance
                {
                    Id = Guid.NewGuid(),
                    CustomerId = CustomerSeeding.OmarKhaledId,
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