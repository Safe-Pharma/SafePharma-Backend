namespace SafePharma.DAL
{
    public static class ChronicConditionSeeding
    {
        public static List<ChronicCondition> GetConditions() => new()
        {
            new ChronicCondition
            {
                Id = Guid.NewGuid(),
                NameEn = "Diabetes",
                NameAr = "السكري",
                CreatedAt = DateTime.UtcNow
            },

            new ChronicCondition
            {
                Id = Guid.NewGuid(),
                NameEn = "Hypertension",
                NameAr = "ضغط الدم المرتفع",
                CreatedAt = DateTime.UtcNow
            },

            new ChronicCondition
            {
                Id = Guid.NewGuid(),
                NameEn = "Asthma",
                NameAr = "الربو",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}
