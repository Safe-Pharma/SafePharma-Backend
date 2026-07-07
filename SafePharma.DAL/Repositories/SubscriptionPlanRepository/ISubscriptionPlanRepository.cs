namespace SafePharma.DAL
{
    public interface ISubscriptionPlanRepository : IGenircRepository<SubscriptionPlan>
    {
        Task<SubscriptionPlan?> GetByTier(string tier);
        Task<IEnumerable<SubscriptionPlan>> GetActiveOrdered();
    }
}