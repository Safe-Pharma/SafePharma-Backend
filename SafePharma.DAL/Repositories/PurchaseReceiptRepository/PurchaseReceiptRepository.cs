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
    }
}
