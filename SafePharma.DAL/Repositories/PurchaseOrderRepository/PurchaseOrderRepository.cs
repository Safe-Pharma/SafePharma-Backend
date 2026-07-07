using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PurchaseOrderRepository : GenircRepository<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<PurchaseOrder?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _db.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Supplier)
                .Include(po => po.Items)
                    .ThenInclude(item => item.Medicine)
                .FirstOrDefaultAsync(po => po.Id == id);
        }
        public async Task<IEnumerable<PurchaseOrder>> GetAllWithSupplierAsync(Guid pharmacyId)
        {
            return await _db.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Supplier)
                .Where(po => po.PharmacyId == pharmacyId)
                .ToListAsync();
        }
    }
}
