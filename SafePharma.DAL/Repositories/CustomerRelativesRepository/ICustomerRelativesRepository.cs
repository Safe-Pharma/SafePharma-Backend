namespace SafePharma.DAL 
{
    public interface ICustomerRelativesRepository:IGenircRepository<CustomerRelative>
    {
        Task<bool> HasPortalAccessAsync(Guid requesterId, Guid targetCustomerId);
        Task<bool> IsFound(Guid requesterId, Guid targetCustomerId);
    }
}