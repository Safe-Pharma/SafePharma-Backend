using SafePharma.DAL;

namespace ecommerce.DAL
{
    public class CustomerChronicCondition
    {
        public Guid CustomerId { get; set; }

        public Guid ChronicConditionId { get; set; }

        public Customer Customer { get; set; } = null!;

        public ChronicCondition ChronicCondition { get; set; } = null!;
    }
}
