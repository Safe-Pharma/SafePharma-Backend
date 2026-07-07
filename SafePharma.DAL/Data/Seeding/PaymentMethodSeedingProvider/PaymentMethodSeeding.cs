using System.Text.Json;

namespace SafePharma.DAL.Data.Seeding.PaymentMethodSeedingProvider
{
    public static class PaymentMethodSeeding
    {
        public static List<PaymentMethod> GetMethods() => new()
        {
            new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodName = "Instapay", IsActive = true, SortOrder = 1,
                FieldsJson = JsonSerializer.Serialize(new[]
                {
                    new { Label = "Handle", Value = "@safepharma" },
                    new { Label = "Name", Value = "SafePharma Systems LLC" }
                })
            },
            new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodName = "Bank Transfer", IsActive = true, SortOrder = 2,
                FieldsJson = JsonSerializer.Serialize(new[]
                {
                    new { Label = "Bank", Value = "Emirates NBD" },
                    new { Label = "Account Name", Value = "SafePharma Systems LLC" },
                    new { Label = "IBAN", Value = "AE07 0331 2345 6789 0123 456" },
                    new { Label = "SWIFT", Value = "EBILAEAD" }
                })
            },
            new PaymentMethod
            {
                Id = Guid.NewGuid(), MethodName = "Vodafone Cash", IsActive = true, SortOrder = 3,
                FieldsJson = JsonSerializer.Serialize(new[]
                {
                    new { Label = "Wallet", Value = "+20 100 555 4488" },
                    new { Label = "Name", Value = "SafePharma Egypt" }
                })
            }
        };
    }
}