namespace SafePharma.DAL
{
    public enum SupplierStatus
    {
        Active,
        Inactive
    }

    public class Supplier : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public SupplierStatus Status { get; set; } = SupplierStatus.Active;

        public decimal Outstanding { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;

        public Guid CountryId { get; set; }
        public Country Country { get; set; } = null!;
    }
}