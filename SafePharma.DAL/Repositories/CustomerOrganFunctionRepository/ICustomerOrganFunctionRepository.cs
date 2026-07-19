namespace SafePharma.DAL
{
    public interface ICustomerOrganFunctionRepository
    {
        Task<IEnumerable<CustomerOrganFunction>> GetForCustomer(Guid customerId);
        Task<CustomerOrganFunction?> GetById(Guid id);
        Task<CustomerOrganFunction?> FindByOrgan(Guid customerId, Guid organId);
        void Add(CustomerOrganFunction entity);
        void Remove(CustomerOrganFunction entity);
    }
}