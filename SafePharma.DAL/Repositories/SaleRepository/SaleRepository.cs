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
        public async Task<IEnumerable<Sale>> GetAllForPharmacy(Guid pharmacyId, SaleStatus? status = null, string? search = null)
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

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.ToLower();
                query = query.Where(s =>
                    s.InvoiceNumber.ToLower().Contains(q) ||
                    (s.Customer != null && s.Customer.Name.ToLower().Contains(q)));
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        // ---- stats ----

        public async Task<decimal> GetTodayTotal(Guid pharmacyId)
        {
            var today = DateTime.UtcNow.Date;

            return await _db.Sales
                .Where(s => s.PharmacyId == pharmacyId
                    && s.Status == SaleStatus.Completed
                    && s.CreatedAt.Date == today)
                .SumAsync(s => (decimal?)s.GrandTotal) ?? 0m;
        }

        public async Task<int> GetCompletedCount(Guid pharmacyId)
        {
            return await _db.Sales
                .CountAsync(s => s.PharmacyId == pharmacyId && s.Status == SaleStatus.Completed);
        }
        public async Task<int> GetCancelledCount(Guid pharmacyId)
        {
            return await _db.Sales
                .CountAsync(s => s.PharmacyId == pharmacyId && s.Status == SaleStatus.Cancelled);
        }

        public async Task<decimal> GetAverageBasket(Guid pharmacyId)
        {
            var completed = _db.Sales
                .Where(s => s.PharmacyId == pharmacyId && s.Status == SaleStatus.Completed);

            var count = await completed.CountAsync();
            if (count == 0) return 0m;

            var sum = await completed.SumAsync(s => s.GrandTotal);
            return Math.Round(sum / count, 2);
        }
    }
}
