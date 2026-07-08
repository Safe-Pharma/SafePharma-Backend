using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class SupplierPaymentRepository : GenircRepository<SupplierPayment>, ISupplierPaymentRepository
    {
        public SupplierPaymentRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<SupplierPayment>> GetHistoryForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<SupplierPayment>()
                .AsNoTracking()
                .Include(p => p.Supplier)
                .Where(p => p.Supplier.PharmacyId == pharmacyId)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
        }

        public async Task<int> CountForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<SupplierPayment>()
                .Where(p => p.Supplier.PharmacyId == pharmacyId)
                .CountAsync();
        }
    }
}