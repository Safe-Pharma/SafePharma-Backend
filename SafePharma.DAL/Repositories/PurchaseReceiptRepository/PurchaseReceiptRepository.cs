using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PurchaseReceiptRepository : GenircRepository<PurchaseReceipt>, IPurchaseReceiptRepository
    {
        public PurchaseReceiptRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<PurchaseReceipt>> GetAllWithItems()
        {
            return await _db.PurchaseReceipts
                .Include(x => x.Items)
                    .ThenInclude(x => x.PurchaseOrderItem)
                        .ThenInclude(x => x.PharmacyMedicine)
                            .ThenInclude(x => x.Medicine)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseReceipt>> GetAllForPharmacy(Guid pharmacyId)
        {
            return await _db.PurchaseReceipts
                .Include(x => x.Items)
                    .ThenInclude(x => x.PurchaseOrderItem)
                        .ThenInclude(x => x.PharmacyMedicine)
                            .ThenInclude(x => x.Medicine)
                .Where(r => r.PurchaseOrder.PharmacyId == pharmacyId)
                .ToListAsync();
        }

        public async Task<PurchaseReceipt?> GetByIdWithDetailsAsync(Guid id, Guid pharmacyId)
        {
            return await _db.PurchaseReceipts
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Items)
                    .ThenInclude(x => x.PurchaseOrderItem)
                        .ThenInclude(x => x.PharmacyMedicine)
                .FirstOrDefaultAsync(r => r.Id == id && r.PurchaseOrder.PharmacyId == pharmacyId);
        }
    }
}