using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PaymentVerificationRepository : GenircRepository<PaymentVerification>, IPaymentVerificationRepository
    {
        public PaymentVerificationRepository(AppDbContext db) : base(db) { }

        public async Task<PaymentVerification?> GetByIdWithSubscription(Guid id)
        {
            return await _db.Set<PaymentVerification>()
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Pharmacy)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PaymentVerification>> GetPendingWithSubscription()
        {
            return await _db.Set<PaymentVerification>()
                .AsNoTracking()
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.Pharmacy)
                .Where(p => p.Status == PaymentVerificationStatus.Pending)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasPendingForSubscription(Guid subscriptionId)
        {
            return await _db.Set<PaymentVerification>()
                .AnyAsync(p => p.SubscriptionId == subscriptionId
                            && p.Status == PaymentVerificationStatus.Pending);
        }
    }
}