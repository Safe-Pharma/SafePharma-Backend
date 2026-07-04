namespace SafePharma.DAL
{
    public static class CountrySeeding
    {
        public static List<Country> GetCountries()
        {
            var uae = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var ksa = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var egypt = Guid.Parse("10000000-0000-0000-0000-000000000003");
            var jordan = Guid.Parse("10000000-0000-0000-0000-000000000004");
            var qatar = Guid.Parse("10000000-0000-0000-0000-000000000005");
            var kuwait = Guid.Parse("10000000-0000-0000-0000-000000000006");
            var bahrain = Guid.Parse("10000000-0000-0000-0000-000000000007");
            var oman = Guid.Parse("10000000-0000-0000-0000-000000000008");

            return new List<Country>
            {
                new Country
                {
                    Id = uae, Name = "United Arab Emirates", Code = "AE",
                    Cities = CitiesOf("Dubai", "Abu Dhabi", "Sharjah", "Ajman",
                        "Ras Al Khaimah", "Fujairah", "Umm Al Quwain", "Al Ain")
                },
                new Country
                {
                    Id = ksa, Name = "Saudi Arabia", Code = "SA",
                    Cities = CitiesOf("Riyadh", "Jeddah", "Dammam", "Mecca", "Medina",
                        "Khobar", "Taif", "Abha", "Jubail", "Tabuk")
                },
                new Country
                {
                    Id = egypt, Name = "Egypt", Code = "EG",
                    Cities = CitiesOf("Cairo", "Alexandria", "Giza", "Ismailia", "Luxor",
                        "Aswan", "Port Said", "Suez", "Mansoura", "Tanta")
                },
                new Country
                {
                    Id = jordan, Name = "Jordan", Code = "JO",
                    Cities = CitiesOf("Amman", "Zarqa", "Irbid", "Aqaba", "Salt",
                        "Madaba", "Karak", "Jerash")
                },
                new Country
                {
                    Id = qatar, Name = "Qatar", Code = "QA",
                    Cities = CitiesOf("Doha", "Al Rayyan", "Al Wakrah", "Al Khor",
                        "Umm Salal", "Lusail")
                },
                new Country
                {
                    Id = kuwait, Name = "Kuwait", Code = "KW",
                    Cities = CitiesOf("Kuwait City", "Hawalli", "Salmiya", "Farwaniya",
                        "Jahra", "Ahmadi")
                },
                new Country
                {
                    Id = bahrain, Name = "Bahrain", Code = "BH",
                    Cities = CitiesOf("Manama", "Riffa", "Muharraq", "Hamad Town",
                        "Isa Town", "Sitra")
                },
                new Country
                {
                    Id = oman, Name = "Oman", Code = "OM",
                    Cities = CitiesOf("Muscat", "Salalah", "Sohar", "Nizwa", "Sur", "Ibri")
                },
            };
        }

        private static List<City> CitiesOf(params string[] names)
        {
            return names.Select(n => new City { Id = Guid.NewGuid(), Name = n }).ToList();
        }
    }
}