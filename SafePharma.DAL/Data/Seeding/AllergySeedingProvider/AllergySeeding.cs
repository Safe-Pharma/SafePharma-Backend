namespace SafePharma.DAL
{
    public static class AllergySeeding
    {
        public static List<Allergy> GetAllergies() => new()
        {
            new Allergy
            {
                Id = Guid.NewGuid(),
                NameEn = "Penicillin",
                NameAr = "البنسلين",
                CreatedAt = DateTime.UtcNow
            },

            new Allergy
            {
                Id = Guid.NewGuid(),
                NameEn = "Dust",
                NameAr = "الغبار",
                CreatedAt = DateTime.UtcNow
            },

            new Allergy
            {
                Id = Guid.NewGuid(),
                NameEn = "Seafood",
                NameAr = "المأكولات البحرية",
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}
