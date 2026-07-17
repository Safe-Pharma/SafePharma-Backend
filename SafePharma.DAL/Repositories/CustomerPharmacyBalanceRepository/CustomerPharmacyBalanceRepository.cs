using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class CustomerPharmacyBalanceRepository : GenircRepository<CustomerPharmacyBalance>, ICustomerPharmacyBalanceRepository
    {
        public CustomerPharmacyBalanceRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<CustomerPharmacyBalance>> GetForCustomer(Guid customerId)
        {
            return await _db.Set<CustomerPharmacyBalance>()
                .AsNoTracking()
                .Include(b => b.Pharmacy)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.TotalPaid)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerPharmacyBalance>> GetForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<CustomerPharmacyBalance>()
                .AsNoTracking()
                .Where(b => b.PharmacyId == pharmacyId)
                .ToListAsync();
        }

        public async Task<CustomerPharmacyBalance?> GetByCustomerAndPharmacy(Guid customerId, Guid pharmacyId)
        {
            return await _db.Set<CustomerPharmacyBalance>()
                .FirstOrDefaultAsync(b => b.CustomerId == customerId && b.PharmacyId == pharmacyId);
        }
    }
}