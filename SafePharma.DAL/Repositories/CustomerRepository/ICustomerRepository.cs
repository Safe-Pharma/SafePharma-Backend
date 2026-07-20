namespace SafePharma.DAL
{
    public interface ICustomerRepository : IGenircRepository<Customer>
    {
        Task<bool> PhoneExists(string phone, Guid? excludeId = null);
        Task<IEnumerable<Customer>> Search(string? query);
        Task<Customer?> GetByIdWithHistory(Guid id);
        Task<Customer?> GetByPhone(string phone);

    }
}
