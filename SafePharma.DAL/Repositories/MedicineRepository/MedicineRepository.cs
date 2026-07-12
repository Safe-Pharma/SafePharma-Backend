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
            // Only global medicines share this uniqueness rule.
            var query = _db.Set<Medicine>()
                .Where(m => m.IsGlobal && m.TradeNameEn == tradeNameEn);

            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Medicine?> GetByTradeNameEn(string tradeNameEn)
        {
            return await _db.Set<Medicine>()
                .FirstOrDefaultAsync(m => m.IsGlobal && m.TradeNameEn == tradeNameEn);
        }

        public async Task<Medicine?> GetLocalByTradeNameEnForPharmacy(Guid pharmacyId, string tradeNameEn)
        {
            return await _db.Set<Medicine>()
                .FirstOrDefaultAsync(m => !m.IsGlobal
                    && m.OwnerPharmacyId == pharmacyId
                    && m.TradeNameEn == tradeNameEn);
        }

        public async Task<IEnumerable<Medicine>> SearchGlobal(string? query)
        {
            var medicines = _db.Set<Medicine>().AsNoTracking().Where(m => m.IsGlobal).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                medicines = medicines.Where(m =>
                    m.TradeNameAr.ToLower().Contains(q) ||
                    m.TradeNameEn.ToLower().Contains(q) ||
                    m.ScientificName.ToLower().Contains(q));
            }

            return await medicines.OrderBy(m => m.TradeNameEn).Take(30).ToListAsync();
        }
    }
}