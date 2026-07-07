using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PaymentMethodRepository : GenircRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(AppDbContext db) : base(db) { }

        public async Task<IEnumerable<PaymentMethod>> GetActiveOrdered()
            => await _db.Set<PaymentMethod>()
                .AsNoTracking()
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();
    }
}