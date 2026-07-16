using ecommerce.DAL;

namespace SafePharma.DAL
{
    public class ChronicCondition
    {
        public Guid Id { get; set; }

        public string NameEn { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public ICollection<CustomerChronicCondition> CustomerChronicConditions { get; set; }
            = new HashSet<CustomerChronicCondition>();
    }
}
