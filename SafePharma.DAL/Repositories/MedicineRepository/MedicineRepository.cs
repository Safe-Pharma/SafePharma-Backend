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
    }
}