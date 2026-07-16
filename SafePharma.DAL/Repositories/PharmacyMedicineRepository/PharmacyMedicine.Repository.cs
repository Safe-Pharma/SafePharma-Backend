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
                .Include(mp => mp.PharmacyMedicineTaxes)
                    .ThenInclude(pmt => pmt.Tax)
                .FirstOrDefaultAsync(mp => mp.MedicineId == medicineId && mp.PharmacyId == pharmacyId);
        }

        public async Task<PharmacyMedicine?> GetByIdAndPharmacy(Guid pharmacyMedicineId, Guid pharmacyId, bool includeDetails = false)
        {
            var query = _db.Set<PharmacyMedicine>().AsQueryable();

            if (includeDetails)
            {
                query = query
                    .Include(mp => mp.PharmacyMedicineTaxes)
                        .ThenInclude(pmt => pmt.Tax)
                    .Include(mp => mp.PharmacyBarcodes);
            }

            return await query.FirstOrDefaultAsync(mp =>
                mp.Id == pharmacyMedicineId && mp.PharmacyId == pharmacyId);
        }

        public async Task<PharmacyMedicine?> GetDetailsByIdAndPharmacy(Guid pharmacyMedicineId, Guid pharmacyId)
        {
            return await _db.Set<PharmacyMedicine>()
                .Include(mp => mp.Medicine)
                    .ThenInclude(m => m!.ManufacturerBarcodes)
                .Include(mp => mp.PharmacyMedicineTaxes)
                    .ThenInclude(pmt => pmt.Tax)
                .Include(mp => mp.PharmacyBarcodes)
                .FirstOrDefaultAsync(mp => mp.Id == pharmacyMedicineId && mp.PharmacyId == pharmacyId);
        }

        public async Task<IEnumerable<PharmacyMedicine>> Search(Guid pharmacyId, string? query, string? category = null, bool includeInactive = false)
        {
            var prices = _db.Set<PharmacyMedicine>()
                .AsNoTracking()
                .Include(mp => mp.Medicine)
                    .ThenInclude(m => m!.ManufacturerBarcodes)
                .Include(mp => mp.PharmacyMedicineTaxes)
                    .ThenInclude(pmt => pmt.Tax)
                .Include(mp => mp.PharmacyBarcodes)
                .Where(mp => mp.PharmacyId == pharmacyId);

            prices = includeInactive ? prices.Where(mp => !mp.IsActive) : prices.Where(mp => mp.IsActive);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                prices = prices.Where(mp =>
                    mp.TradeNameAr.ToLower().Contains(q) ||
                    mp.TradeNameEn.ToLower().Contains(q) ||
                    mp.ScientificName.ToLower().Contains(q) ||
                    mp.SKU.ToLower().Contains(q) ||
                    mp.PharmacyBarcodes.Any(b => b.Barcode.ToLower().Contains(q)) ||
                    (mp.Medicine != null && mp.Medicine.ManufacturerBarcodes.Any(b => b.Barcode.ToLower().Contains(q))));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                prices = prices.Where(mp => mp.Category == category);
            }

            return await prices.OrderBy(mp => mp.TradeNameEn).ToListAsync();
        }

        public async Task<IEnumerable<PharmacyMedicine>> GetAllForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<PharmacyMedicine>()
                .AsNoTracking()
                .Where(mp => mp.PharmacyId == pharmacyId)
                .ToListAsync();
        }

        public async Task<int> GetHighestAutoSkuNumber(Guid pharmacyId, string prefix)
        {
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

        public async Task<bool> TradeNameExistsForPharmacy(Guid pharmacyId, string tradeNameEn, Guid? excludeId = null)
        {
            var query = _db.Set<PharmacyMedicine>()
                .Where(pm => pm.PharmacyId == pharmacyId && pm.TradeNameEn.ToLower() == tradeNameEn.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(pm => pm.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<(IEnumerable<PharmacyMedicine> Items, int TotalCount)> SearchAsync(
    Guid pharmacyId, string? query, int pageNumber, int pageSize)
        {
            var baseQuery = _db.Set<PharmacyMedicine>()
                .AsNoTracking()
                .Include(pm => pm.PharmacyBarcodes)
                .Where(pm => pm.PharmacyId == pharmacyId && pm.IsActive);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim().ToLower();
                baseQuery = baseQuery.Where(pm =>
                    pm.TradeNameAr.ToLower().Contains(q) ||
                    pm.TradeNameEn.ToLower().Contains(q) ||
                    pm.ScientificName.ToLower().Contains(q) ||
                    pm.SKU.ToLower().Contains(q) ||
                    pm.PharmacyBarcodes.Any(b => b.Barcode.ToLower().Contains(q)));
            }

            var totalCount = await baseQuery.CountAsync();

            var items = await baseQuery
                .OrderBy(pm => pm.TradeNameEn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
