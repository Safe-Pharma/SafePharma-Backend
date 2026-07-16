namespace SafePharma.DAL
{
    public class CustomerAllergy
    {
        public Guid CustomerId { get; set; }

        public Guid AllergyId { get; set; }

        public Customer Customer { get; set; } = null!;

        public Allergy Allergy { get; set; } = null!;
    }
}
