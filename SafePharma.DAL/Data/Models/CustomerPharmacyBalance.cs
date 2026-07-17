namespace SafePharma.DAL
{
    // How much a customer has paid AT A SPECIFIC PHARMACY. Customer is global, so this
    // is the per-pharmacy breakdown — there is no single "lifetime total" across pharmacies.
    public class CustomerPharmacyBalance
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;

        public decimal TotalPaid { get; set; }
        public DateTime? LastPaymentAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}