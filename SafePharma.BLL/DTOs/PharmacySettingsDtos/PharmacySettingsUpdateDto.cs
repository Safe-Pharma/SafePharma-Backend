using Microsoft.AspNetCore.Http;

namespace SafePharma.BLL
{
    public class PharmacySettingsUpdateDto
    {
        public string Name { get; set; }
        public IFormFile? LogoFile { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? TaxRegistrationNumber { get; set; }
    }
}
