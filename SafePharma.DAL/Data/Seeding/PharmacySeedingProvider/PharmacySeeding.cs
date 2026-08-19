namespace SafePharma.DAL
{
    public static class PharmacySeeding
    {
        public static List<Subscription> GetSubscriptionsWithPharmacies()
        {
            return new List<Subscription>
            {
                new Subscription
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                    PlanTier = "Professional",
                    BillingCycle = "monthly",
                    Status = SubscriptionStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    ApprovedAt = DateTime.UtcNow.AddDays(-29),
                    Pharmacy = new Pharmacy
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                        Name = "MediRx Pharmacy",
                        TaxNumber = "100223344556600",
                        CommercialRegistration = "CR-753420",
                        Address = "Al Wasl Road, Building 12, Jumeirah",
                        Country = "United Arab Emirates",
                        City = "Dubai",
                        Phone = "+971501234567",
                        BusinessEmail = "ops@medirxpharmacy.com",
                        NumberOfBranches = 3,
                        PreferredLanguage = "English",
                        TimeZone = "(GMT+4) Gulf Standard Time",
                        isActive=false,
                    }
                },
                new Subscription
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    PlanTier = "Starter",
                    BillingCycle = "yearly",
                    Status = SubscriptionStatus.AwaitingPayment,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    ApprovedAt = null,
                    Pharmacy = new Pharmacy
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                        Name = "Al Shifa Pharmacy",
                        TaxNumber = null,
                        CommercialRegistration = null,
                        Address = "Corniche Road, Downtown",
                        Country = "Egypt",
                        City = "Cairo",
                        Phone = "+201012345678",
                        BusinessEmail = "contact@alshifa-pharmacy.com",
                        NumberOfBranches = 1,
                        PreferredLanguage = "Arabic",
                        TimeZone = "(GMT+2) Eastern European Time",
                        isActive=false,

                    }
                },
                new Subscription
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                    PlanTier = "Enterprise",
                    BillingCycle = "yearly",
                    Status = SubscriptionStatus.AwaitingPayment,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    ApprovedAt = DateTime.UtcNow.AddDays(-5),
                    Pharmacy = new Pharmacy
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                        Name = "Nour Al Hayat Pharmacy Group",
                        TaxNumber = "300012345600003",
                        CommercialRegistration = "CR-4471029",
                        Address = "King Fahd Road, Al Olaya",
                        Country = "Saudi Arabia",
                        City = "Riyadh",
                        Phone = "+966501234567",
                        BusinessEmail = "info@nouralhayat.sa",
                        NumberOfBranches = 12,
                        PreferredLanguage = "Arabic",
                        TimeZone = "(GMT+3) Arabia Standard Time",
                        isActive=false,

                    }
                },
            };
        }
    }
}