using System.Text.Json;

namespace SafePharma.DAL.Data.Seeding.SubscriptionPlanSeedingProvider
{
    public static class SubscriptionPlanSeeding
    {
        public static List<SubscriptionPlan> GetPlans() => new()
        {
            new SubscriptionPlan
            {
                Id = Guid.NewGuid(), Tier = "Starter", DisplayName = "Starter",
                MonthlyPrice = 49, YearlyPrice = 490, Currency = "EGP",
                FeaturesJson = JsonSerializer.Serialize(new[] {"5 users", "Inventory + POS" }),
                IsActive = true, SortOrder = 1
            },
            new SubscriptionPlan
            {
                Id = Guid.NewGuid(), Tier = "Professional", DisplayName = "Professional",
                MonthlyPrice = 129, YearlyPrice = 1290, Currency = "EGP",
                FeaturesJson = JsonSerializer.Serialize(new[] {"Unlimited users", "All modules" }),
                IsActive = true, SortOrder = 2
            },
            new SubscriptionPlan
            {
                Id = Guid.NewGuid(), Tier = "Enterprise", DisplayName = "Enterprise",
                MonthlyPrice = 0, YearlyPrice = 0, Currency = "EGP",   // 0 = "contact us" — handle on the FE
                FeaturesJson = JsonSerializer.Serialize(new[] {"SSO", "Dedicated CSM" }),
                IsActive = true, SortOrder = 3
            }
        };
    }
}