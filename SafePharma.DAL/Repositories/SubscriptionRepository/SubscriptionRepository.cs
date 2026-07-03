using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class SubscriptionRepository : GenircRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<Subscription?> GetByIdWithPharmacy(Guid id)
        {
            return await _db.Subscriptions
                .Include(s => s.Pharmacy)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetAllWithPharmacy()
        {
            return await _db.Subscriptions
                .AsNoTracking()
                .Include(s => s.Pharmacy)
                .ToListAsync();
        }
    }
}