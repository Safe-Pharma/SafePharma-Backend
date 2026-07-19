using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class CustomerChronicConditionRepository : ICustomerChronicConditionRepository
    {
        private readonly AppDbContext _db;

        public CustomerChronicConditionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CustomerChronicCondition>> GetForCustomer(Guid customerId)
        {
            return await _db.Set<CustomerChronicCondition>()
                .AsNoTracking()
                .Include(x => x.ChronicCondition)
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<CustomerChronicCondition?> Find(Guid customerId, Guid chronicConditionId)
        {
            return await _db.Set<CustomerChronicCondition>()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.ChronicConditionId == chronicConditionId);
        }

        public void Add(CustomerChronicCondition entity) => _db.Set<CustomerChronicCondition>().Add(entity);

        public void Remove(CustomerChronicCondition entity) => _db.Set<CustomerChronicCondition>().Remove(entity);
    }
}