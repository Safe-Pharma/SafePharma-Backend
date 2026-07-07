namespace SafePharma.DAL
{
    public class Pharmacy : IAuditableEntity
    {
        public Guid Id { get; set; }
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
        public DateTime CreatedAt { get; set; }

        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public PharmacySettings? PharmacySettings { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        = new HashSet<PurchaseOrder>();

        public DateTime? UpdatedAt { get; set; }
    }
}