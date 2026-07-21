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
        public async Task<IEnumerable<IGrouping<Guid, Batch>>> GetBatchesGroupByhMedicine()
        {
            return await _db.Set<Batch>().Include(b=>b.Medicine).ThenInclude(b=>b.Medicine).GroupBy(b => b.MedicineId).ToListAsync();
        }
        public async Task<IEnumerable<Batch>> GetBatchesByhMedicineId(Guid MId)
        {
            return await _db.Set<Batch>().Select(b => b).Where(m => m.Id == MId).ToListAsync();
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
            return await _db.Set<Batch>()
                .AsNoTracking()
                .Include(b => b.Medicine)
                .Where(b => b.QuantityRemaining > 0)
                .ToListAsync();
        }


        public async Task<int> GetAvailableQuantity(Guid pharmacyMedicineId)
        {
            var today = DateTime.UtcNow.Date;

            return await _db.Set<Batch>()
                .Where(b =>
                    b.MedicineId == pharmacyMedicineId &&
                    b.QuantityRemaining > 0 &&
                    b.ExpiryDate > today)
                .SumAsync(b => b.QuantityRemaining);
        }

    }


}