using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class SubscriptionPlanRepository : GenircRepository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        public SubscriptionPlanRepository(AppDbContext db) : base(db) { }

        public async Task<SubscriptionPlan?> GetByTier(string tier)
            => await _db.Set<SubscriptionPlan>().FirstOrDefaultAsync(p => p.Tier == tier);

        public async Task<IEnumerable<SubscriptionPlan>> GetActiveOrdered()
            => await _db.Set<SubscriptionPlan>()
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();
    }
}