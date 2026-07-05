namespace SafePharma.DAL
{
    public class PharmacySettings
    {
        public Guid Id { get; set; }
        public Guid? PharmacyId { get; set; }
        public Pharmacy? Pharmacy { get; set; }
        public string Name { get; set; }
        public string? LogoUrl { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Phone { get; set; }
        public string? TaxRegistrationNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
