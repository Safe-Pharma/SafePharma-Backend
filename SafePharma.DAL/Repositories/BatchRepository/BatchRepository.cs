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
    }
}
