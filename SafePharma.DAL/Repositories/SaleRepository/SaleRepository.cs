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
    }
    }
