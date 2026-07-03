
namespace SafePharma.BLL
{
    public class PharmacyInfoDto
    {
        public string Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? TaxNumber { get; set; }
        public string? CommercialRegistration { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string BusinessEmail { get; set; }
        public int NumberOfBranches { get; set; }
        public string PreferredLanguage { get; set; }
        public string TimeZone { get; set; }
    }
}
