namespace SafePharma.DAL
{
    public enum TaxStatus
    {
        Active,
        Inactive
    }

    public class Tax : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public TaxStatus Status { get; set; } = TaxStatus.Active;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
