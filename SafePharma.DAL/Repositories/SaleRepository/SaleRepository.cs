using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class SaleRepository : GenircRepository<Sale>, ISaleRepository
    {
        public SaleRepository(AppDbContext db) : base(db)
        {
        }
        public async Task<Sale?> GetByIdWithItemsAsync(Guid saleId)
        {
            return await _db.Sales
                .Include(s => s.Customer) 
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Batch)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.PharmacyMedicine)
                        .ThenInclude(pm => pm.Medicine)
                .FirstOrDefaultAsync(s => s.Id == saleId);
        }
        public async Task<IEnumerable<Sale>> GetAllForPharmacy(Guid pharmacyId, SaleStatus? status = null)
        {
            var query = _db.Sales
                .AsNoTracking()
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Batch)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.PharmacyMedicine)
                        .ThenInclude(pm => pm.Medicine)
                .Where(s => s.PharmacyId == pharmacyId);

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
