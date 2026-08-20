using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL
{
    public class BatchRepository : GenircRepository<Batch>, IBatchRepository
    {
        public BatchRepository(AppDbContext db) : base(db)
        {

        }
        public async Task<Batch?> GetByIdForPharmacyAsync(Guid batchId, Guid pharmacyId)
        {
            return await _db.Set<Batch>()
                        .FirstOrDefaultAsync(b =>
                            b.Id == batchId &&
                            b.PharmacyId == pharmacyId);
        }
        public async Task<PharmacyMedicine?> GetByIdForPharmacyMedecineAsync(
                                                        Guid id,
                                                        Guid pharmacyId)
                                                            {
                                                                return await _db.Set<PharmacyMedicine>()
                                                                    .FirstOrDefaultAsync(pm =>
                                                                        pm.Id == id &&
                                                                        pm.PharmacyId == pharmacyId);
                                                            }
        public async Task<PurchaseReceiptItem?> GetByIdForRecieptAsync(
    Guid id,
    Guid pharmacyId)
        {
            return await _db.Set<PurchaseReceiptItem>()
                .Include(x => x.PharmacyMedicine)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.PharmacyMedicine.PharmacyId == pharmacyId);
        }

        public async Task<IEnumerable<IGrouping<Guid, Batch>>> GetBatchesGroupByMedicineAsync(Guid pharmacyId)
        {
            return await _db.Set<Batch>()
                .AsNoTracking()
                .Where(b => b.PharmacyId == pharmacyId && !b.IsDeleted)  
                .Include(b => b.Medicine)
                .ThenInclude(b => b.Medicine)
                .GroupBy(b => b.MedicineId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Batch>> GetBatchesByMedicineId(Guid medicineId)
        {
            return await _db.Set<Batch>()
                .Where(b => b.MedicineId == medicineId)
                .ToListAsync();
        }
        public async Task<IEnumerable<StockAggregate>> GetStockAggregates(IEnumerable<Guid> pharmacyMedicineIds, int expiringSoonDays = 90)
        {
            var ids = pharmacyMedicineIds.ToList();
            if (ids.Count == 0) return Enumerable.Empty<StockAggregate>();

            var today = DateTime.UtcNow.Date;
            var expiryThreshold = today.AddDays(expiringSoonDays);

            return await _db.Set<Batch>()
                .AsNoTracking()
                .Where(b => ids.Contains(b.MedicineId))
                .GroupBy(b => b.MedicineId)
                .Select(g => new StockAggregate
                {
                    PharmacyMedicineId = g.Key,
                    TotalStock = g.Sum(b => b.QuantityReceived),
                    AvailableQuantity = g.Sum(b => b.QuantityRemaining),
                    BatchCount = g.Count(),
                    ExpiringSoon = g.Count(b => b.QuantityRemaining > 0 && b.ExpiryDate > today && b.ExpiryDate <= expiryThreshold),
                })
                .ToListAsync();


        }
        public async Task<Batch?> GetByPurchaseReceiptItemId(Guid purchaseReceiptItemId)
        {
            return await _db.Set<Batch>()
                .FirstOrDefaultAsync(b => b.PurchaseReceiptItemId == purchaseReceiptItemId);

        }
        public async Task<Batch?> GetNearestExpiryBatchAsync(Guid pharmacyMedicineId)
        {
            return await _db.Set<Batch>()
                .Where(b => b.MedicineId == pharmacyMedicineId && b.QuantityRemaining > 0)
                .OrderBy(b => b.ExpiryDate)
                .FirstOrDefaultAsync();
        }

        
        public async Task<IEnumerable<Batch>> GetBatchesForExpiryNotifications()
        {
            var today = DateTime.UtcNow.Date;
            return await _db.Set<Batch>()
                .AsNoTracking()
                .Include(b => b.Medicine)
                .Where(b =>
                    b.QuantityRemaining > 0 &&
                    b.ExpiryDate <= today.AddDays(90))
                .ToListAsync();
        }


        public async Task<int> GetAvailableQuantity(
    Guid pharmacyMedicineId,
    Guid pharmacyId)
        {
            var today = DateTime.UtcNow.Date;

            return await _db.Set<Batch>()
                .Where(b =>
                    b.PharmacyId == pharmacyId &&
                    b.MedicineId == pharmacyMedicineId &&
                    b.QuantityRemaining > 0 &&
                    b.ExpiryDate > today)
                .SumAsync(b => b.QuantityRemaining);
        }

    }


}