using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PurchaseReceiptItemRepository : GenircRepository<PurchaseReceiptItem>, IPurchaseReceiptItemRepository
    {
        public PurchaseReceiptItemRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<PurchaseReceiptItem?> GetByIdForPharmacy(Guid id, Guid pharmacyId)
        {
            return await _db.PurchaseReceiptItems
                .Include(x => x.PurchaseReceipt)
                    .ThenInclude(r => r.PurchaseOrder)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.PurchaseReceipt.PurchaseOrder.PharmacyId == pharmacyId);
        }
    }
}