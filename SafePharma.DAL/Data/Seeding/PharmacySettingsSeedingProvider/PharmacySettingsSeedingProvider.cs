namespace SafePharma.DAL
{
    public class PharmacySettingsSeedingProvider
    {
        public static List<PharmacySettings> GetDefaultPharmacySettings()
        {
            return new List<PharmacySettings>
            {
                new PharmacySettings
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                    PharmacyId = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new PharmacySettings
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                    PharmacyId = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new PharmacySettings
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                    PharmacyId = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
            };
        }
    }
}