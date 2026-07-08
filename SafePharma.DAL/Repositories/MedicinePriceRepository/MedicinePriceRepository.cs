using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class MedicinePriceRepository : GenircRepository<MedicinePrice>, IMedicinePriceRepository
    {
        public MedicinePriceRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<MedicinePrice?> GetByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId)
        {
            return await _db.Set<MedicinePrice>()
                .Include(mp => mp.Medicine)
                .Include(mp => mp.Tax)
                .FirstOrDefaultAsync(mp => mp.MedicineId == medicineId && mp.PharmacyId == pharmacyId);
        }

        public async Task<IEnumerable<MedicinePrice>> Search(Guid pharmacyId, string? query, string? category = null)
        {
            var prices = _db.Set<MedicinePrice>()
                .AsNoTracking()
                .Include(mp => mp.Medicine)
                .Include(mp => mp.Tax)
                .Where(mp => mp.PharmacyId == pharmacyId);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                prices = prices.Where(mp =>
                    mp.Medicine.TradeNameAr.ToLower().Contains(q) ||
                    mp.Medicine.TradeNameEn.ToLower().Contains(q) ||
                    mp.Medicine.ScientificName.ToLower().Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                prices = prices.Where(mp => mp.Medicine.Category == category);
            }

            return await prices.OrderBy(mp => mp.Medicine.TradeNameEn).ToListAsync();
        }

        public async Task<IEnumerable<MedicinePrice>> GetAllForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<MedicinePrice>()
                .AsNoTracking()
                .Include(mp => mp.Medicine)
                .Where(mp => mp.PharmacyId == pharmacyId)
                .ToListAsync();
        }
    }
}