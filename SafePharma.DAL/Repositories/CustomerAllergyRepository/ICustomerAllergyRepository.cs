namespace SafePharma.DAL
{
    // Composite-key join table (CustomerId + AllergyId) — no single Id, so this doesn't
    // extend IGenircRepository<T> (its GetById(Guid) wouldn't make sense here).
    public interface ICustomerAllergyRepository
    {
        Task<IEnumerable<CustomerAllergy>> GetForCustomer(Guid customerId);
        Task<CustomerAllergy?> Find(Guid customerId, Guid allergyId);
        void Add(CustomerAllergy entity);
        void Remove(CustomerAllergy entity);
    }
}