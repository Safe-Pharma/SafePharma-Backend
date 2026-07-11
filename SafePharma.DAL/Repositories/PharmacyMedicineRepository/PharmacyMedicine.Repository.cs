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
            
            prices = includeInactive? prices.Where(mp => !mp.IsActive): prices.Where(mp => mp.IsActive);
                
                

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
        public async Task<int> GetHighestAutoSkuNumber(Guid pharmacyId, string prefix)
        {
            // Pull only this pharmacy's SKUs that match the auto-generated shape, then parse in memory —
            // avoids fragile string-to-int parsing inside SQL translation.
            var skus = await _db.Set<PharmacyMedicine>()
                .Where(pm => pm.PharmacyId == pharmacyId && pm.SKU.StartsWith(prefix))
                .Select(pm => pm.SKU)
                .ToListAsync();

            var max = 0;
            foreach (var sku in skus)
            {
                var suffix = sku.Substring(prefix.Length);
                if (int.TryParse(suffix, out var n) && n > max)
                {
                    max = n;
                }
            }
            return max;
        }

        public async Task<bool> SkuExistsForPharmacy(Guid pharmacyId, string sku, Guid? excludeId = null)
        {
            var query = _db.Set<PharmacyMedicine>()
                .Where(pm => pm.PharmacyId == pharmacyId && pm.SKU == sku);

            if (excludeId.HasValue)                                
            {
                query = query.Where(pm => pm.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<PharmacyMedicine?> GetByIdAndPharmacy(Guid pharmacyMedicineId, Guid pharmacyId)
        {
            return await _db.Set<PharmacyMedicine>()
                .FirstOrDefaultAsync(x =>
                    x.Id == pharmacyMedicineId &&
                    x.PharmacyId == pharmacyId);
        }    
    }
}