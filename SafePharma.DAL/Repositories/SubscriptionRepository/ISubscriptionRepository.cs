namespace SafePharma.DAL
{
    public interface ISubscriptionRepository : IGenircRepository<Subscription>
    {
        Task<Subscription?> GetByIdWithPharmacy(Guid id);
        Task<IEnumerable<Subscription>> GetAllWithPharmacy();
    }
}