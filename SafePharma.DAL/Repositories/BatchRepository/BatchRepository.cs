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
           return await _db.Set<Batch>().GroupBy(b => b.MedicineId).ToListAsync();
        }
        public async Task<IEnumerable<Batch>> GetBatchesByhMedicineId(Guid MId)
        {
            return await _db.Set<Batch>().Select(b=>b).Where(m=>m.Id==MId).ToListAsync();
        }
        public async Task<IEnumerable<StockAggregate>> GetStockAggregates(IEnumerable<Guid> pharmacyMedicineIds)
        {
            var ids = pharmacyMedicineIds.ToList();
            if (ids.Count == 0) return Enumerable.Empty<StockAggregate>();

            return await _db.Set<Batch>()
                .AsNoTracking()
                .Where(b => ids.Contains(b.MedicineId))
                .GroupBy(b => b.MedicineId)
                .Select(g => new StockAggregate
                {
                    PharmacyMedicineId = g.Key,
                    AvailableQuantity = g.Sum(b => b.QuantityRemaining),
                    BatchCount = g.Count()
                })
                .ToListAsync();
        }
        public async Task<Batch?> GetByPurchaseReceiptItemId(Guid purchaseReceiptItemId)
        {
            return await _db.Set<Batch>()
                .FirstOrDefaultAsync(b => b.PurchaseReceiptItemId == purchaseReceiptItemId);
        }
    }


}
