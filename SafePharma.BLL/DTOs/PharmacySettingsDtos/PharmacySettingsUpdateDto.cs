namespace SafePharma.BLL
{
    public class PharmacySettingsUpdateDto
    {
        public string Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Phone { get; set; }
        public string? TaxRegistrationNumber { get; set; }
    }
}
