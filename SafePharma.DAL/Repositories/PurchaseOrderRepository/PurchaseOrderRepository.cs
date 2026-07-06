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
                .Include(po => po.PurchaseOrdersItems)
                    .ThenInclude(item => item.Medicine)
                .FirstOrDefaultAsync(po => po.Id == id);
        }
        public async Task<IEnumerable<PurchaseOrder>> GetAllWithSupplierAsync()
        {
            return await _db.PurchaseOrders
                .AsNoTracking()
                .Include(po => po.Supplier)
                .ToListAsync();
        }
    }
}
