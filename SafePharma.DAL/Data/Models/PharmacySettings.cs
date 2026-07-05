namespace SafePharma.DAL
{
    public class PharmacySettings
    {
        public Guid Id { get; set; }

        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
