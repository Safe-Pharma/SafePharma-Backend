using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PharmacyMedicineRepository : GenircRepository<PharmacyMedicine>, IPharmacyMedicineRepository
    {
        public PharmacyMedicineRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<PharmacyMedicine?> GetByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId)
        {
            return await _db.Set<PharmacyMedicine>()
                .Include(mp => mp.Medicine)
                .Include(mp => mp.PharmacyMedicineTaxes)
                    .ThenInclude(pmt => pmt.Tax)
                .FirstOrDefaultAsync(mp => mp.MedicineId == medicineId && mp.PharmacyId == pharmacyId);
        }

        public async Task<PharmacyMedicine?> GetDetailsByMedicineAndPharmacy(Guid medicineId, Guid pharmacyId)
        {
            return await _db.Set<PharmacyMedicine>()
                .Include(mp => mp.Medicine)
                    .ThenInclude(m => m.ManufacturerBarcodes)
                .Include(mp => mp.PharmacyMedicineTaxes)
                    .ThenInclude(pmt => pmt.Tax)
                .Include(mp => mp.PharmacyBarcodes)
                .FirstOrDefaultAsync(mp => mp.MedicineId == medicineId && mp.PharmacyId == pharmacyId);
        }

        public async Task<IEnumerable<PharmacyMedicine>> Search(Guid pharmacyId, string? query, string? category = null, bool includeInactive = false)
        {
            var prices = _db.Set<PharmacyMedicine>()
                .AsNoTracking()
                .Include(mp => mp.Medicine)
                    .ThenInclude(m => m.ManufacturerBarcodes)
                .Include(mp => mp.PharmacyMedicineTaxes)
                    .ThenInclude(pmt => pmt.Tax)
                .Include(mp => mp.PharmacyBarcodes)
                .Where(mp => mp.PharmacyId == pharmacyId);

            if (!includeInactive)
            {
                prices = prices.Where(mp => mp.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                prices = prices.Where(mp =>
                    mp.Medicine.TradeNameAr.ToLower().Contains(q) ||
                    mp.Medicine.TradeNameEn.ToLower().Contains(q) ||
                    mp.Medicine.ScientificName.ToLower().Contains(q) ||
                    mp.SKU.ToLower().Contains(q) ||
                    mp.PharmacyBarcodes.Any(b => b.Barcode.ToLower().Contains(q)) ||
                    mp.Medicine.ManufacturerBarcodes.Any(b => b.Barcode.ToLower().Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                prices = prices.Where(mp => mp.Medicine.Category == category);
            }

            return await prices.OrderBy(mp => mp.Medicine.TradeNameEn).ToListAsync();
        }

        public async Task<IEnumerable<PharmacyMedicine>> GetAllForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<PharmacyMedicine>()
                .AsNoTracking()
                .Include(mp => mp.Medicine)
                .Where(mp => mp.PharmacyId == pharmacyId)
                .ToListAsync();
        }
    }
}