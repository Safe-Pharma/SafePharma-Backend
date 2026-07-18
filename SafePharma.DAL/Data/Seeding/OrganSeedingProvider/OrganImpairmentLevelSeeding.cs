namespace SafePharma.DAL
{
    public static class OrganImpairmentLevelSeeding
    {
        public static List<OrganImpairmentLevel> GetLevels() => new()
        {
            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Normal",
                NameAr = "طبيعي"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Mild",
                NameAr = "خفيف"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Moderate",
                NameAr = "متوسط"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Severe",
                NameAr = "شديد"
            }
        };
    }
}
