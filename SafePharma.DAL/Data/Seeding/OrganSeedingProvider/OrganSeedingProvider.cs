namespace SafePharma.DAL
{
    public static class OrganSeeding
    {
        public static List<Organ> GetOrgans() => new()
        {
            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Kidney",
                NameAr = "الكلى"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Liver",
                NameAr = "الكبد"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Heart",
                NameAr = "القلب"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Lung",
                NameAr = "الرئة"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Eye",
                NameAr = "العين"
            },

            new()
            {
                Id = Guid.NewGuid(),
                NameEn = "Ear",
                NameAr = "الأذن"
            }
        };
    }
}
