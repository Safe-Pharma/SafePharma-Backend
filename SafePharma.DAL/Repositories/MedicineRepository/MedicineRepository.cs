using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class MedicineRepository : GenircRepository<Medicine>, IMedicineRepository
    {
        public MedicineRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> TradeNameExists(string tradeNameEn, Guid? excludeId = null)
        {
            var query = _db.Set<Medicine>().Where(m => m.TradeNameEn.ToLower() == tradeNameEn.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Medicine?> GetByTradeNameEn(string tradeNameEn)
        {
            return await _db.Set<Medicine>()
                .FirstOrDefaultAsync(m => m.TradeNameEn.ToLower() == tradeNameEn.ToLower());
        }

        public async Task<IEnumerable<Medicine>> SearchGlobal(string? query)
        {
            var medicines = _db.Set<Medicine>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();
                medicines = medicines.Where(m =>
                    m.TradeNameAr.Contains(query) ||
                    m.TradeNameEn.Contains(query) ||
                    m.ScientificName.Contains(query));
                // TODO: once the Barcode table exists, add:
                // || m.Barcodes.Any(b => b.Code.Contains(q))
                // No other layer needs to change for that — only this Where clause.
            }

            return await medicines.OrderBy(m => m.TradeNameEn).Take(30).ToListAsync();
        }
    }
}