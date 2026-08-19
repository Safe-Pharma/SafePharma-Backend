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

        public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(
       Guid customerId,
       string? search = null,
       Guid? pharmacyId = null,
       SaleStatus? status = null,
       DateTime? from = null,
       DateTime? to = null,
       int page = 1,
       int pageSize = 10)
        {
            var query = _db.Sales
                .AsNoTracking()
                .Include(s => s.Customer)
                .Include(s => s.Pharmacy)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Batch)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.PharmacyMedicine)
                        .ThenInclude(pm => pm.Medicine)
                .Where(s => s.CustomerId == customerId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();

                query = query.Where(s =>
                    s.InvoiceNumber.ToLower().Contains(q) ||
                    (s.Pharmacy != null && s.Pharmacy.Name.ToLower().Contains(q)));
            }

            if (pharmacyId.HasValue)
            {
                query = query.Where(s => s.PharmacyId == pharmacyId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            if (from.HasValue)
            {
                var fromDate = from.Value.Date;
                query = query.Where(s => s.CreatedAt >= fromDate);
            }

            if (to.HasValue)
            {
                var toDate = to.Value.Date.AddDays(1);
                query = query.Where(s => s.CreatedAt < toDate);
            }

            return await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Sale?> GetByIdWithItemsAndCustomerIdAsync(Guid saleId, Guid customerId)
        {
            return await _db.Sales
                .AsNoTracking()
                .Include(s => s.Customer)
                .Include(s => s.Pharmacy)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Batch)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.PharmacyMedicine)
                        .ThenInclude(pm => pm.Medicine)
                .FirstOrDefaultAsync(s =>
                    s.Id == saleId &&
                    s.CustomerId == customerId);
        }

        // ---- dashboard ----

        public async Task<IEnumerable<(DateTime Date, decimal Total, int OrderCount)>> GetDailyTotals(Guid pharmacyId, int days)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

            var rows = await _db.Sales
                .AsNoTracking()
                .Where(s => s.PharmacyId == pharmacyId
                    && s.Status == SaleStatus.Completed
                    && s.CreatedAt.Date >= startDate)
                .GroupBy(s => s.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(s => s.GrandTotal), OrderCount = g.Count() })
                .ToListAsync();

            var byDate = rows.ToDictionary(r => r.Date);

            // fill in zero-value days so the trend line/chart doesn't have gaps
            var result = new List<(DateTime Date, decimal Total, int OrderCount)>();
            for (var d = startDate; d <= DateTime.UtcNow.Date; d = d.AddDays(1))
            {
                if (byDate.TryGetValue(d, out var row))
                    result.Add((d, row.Total, row.OrderCount));
                else
                    result.Add((d, 0m, 0));
            }

            return result;
        }

        public async Task<IEnumerable<(string Category, decimal Revenue)>> GetCategoryRevenue(Guid pharmacyId)
        {
            var rows = await _db.SaleItems
                .AsNoTracking()
                .Where(si => si.Sale.PharmacyId == pharmacyId && si.Sale.Status == SaleStatus.Completed)
                .GroupBy(si => si.PharmacyMedicine.Category)
                .Select(g => new { Category = g.Key, Revenue = g.Sum(si => si.LineTotal) })
                .ToListAsync();

            return rows.Select(r => (
                string.IsNullOrWhiteSpace(r.Category) ? "Uncategorized" : r.Category,
                r.Revenue));
        }
    }
}