namespace SafePharma.DAL
{
    public class CustomerOrganFunction
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Guid OrganId { get; set; }

        public Guid OrganImpairmentLevelId { get; set; }

        public DateTime RecordedAt { get; set; }

        public Customer Customer { get; set; } = null!;

        public Organ Organ { get; set; } = null!;

        public OrganImpairmentLevel OrganImpairmentLevel { get; set; } = null!;
    }
}
