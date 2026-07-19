namespace SafePharma.DAL
{
    public interface ICustomerChronicConditionRepository
    {
        Task<IEnumerable<CustomerChronicCondition>> GetForCustomer(Guid customerId);
        Task<CustomerChronicCondition?> Find(Guid customerId, Guid chronicConditionId);
        void Add(CustomerChronicCondition entity);
        void Remove(CustomerChronicCondition entity);
    }
}