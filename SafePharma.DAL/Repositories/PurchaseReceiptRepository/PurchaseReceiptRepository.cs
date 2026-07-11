using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PurchaseReceiptRepository : GenircRepository<PurchaseReceipt>, IPurchaseReceiptRepository
    {
        public PurchaseReceiptRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<PurchaseReceipt>> GetAllForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<PurchaseReceipt>()
                .AsNoTracking()
                .Include(r => r.Items)
                .Where(r => r.PurchaseOrder.PharmacyId == pharmacyId)
                .OrderByDescending(r => r.ReceivedAt)
                .ToListAsync();
        }

        public async Task<PurchaseReceipt?> GetByIdForPharmacy(Guid pharmacyId, Guid id)
        {
            return await _db.Set<PurchaseReceipt>()
                .AsNoTracking()
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == id && r.PurchaseOrder.PharmacyId == pharmacyId);
        }

    }
}
