using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class CustomerOrganFunctionRepository : ICustomerOrganFunctionRepository
    {
        private readonly AppDbContext _db;

        public CustomerOrganFunctionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CustomerOrganFunction>> GetForCustomer(Guid customerId)
        {
            return await _db.Set<CustomerOrganFunction>()
                .AsNoTracking()
                .Include(x => x.Organ)
                .Include(x => x.OrganImpairmentLevel)
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<CustomerOrganFunction?> GetById(Guid id)
        {
            return await _db.Set<CustomerOrganFunction>()
                .Include(x => x.Organ)
                .Include(x => x.OrganImpairmentLevel)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // There's a unique index on (CustomerId, OrganId) — one impairment record per organ per customer.
        public async Task<CustomerOrganFunction?> FindByOrgan(Guid customerId, Guid organId)
        {
            return await _db.Set<CustomerOrganFunction>()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.OrganId == organId);
        }

        public void Add(CustomerOrganFunction entity) => _db.Set<CustomerOrganFunction>().Add(entity);

        public void Remove(CustomerOrganFunction entity) => _db.Set<CustomerOrganFunction>().Remove(entity);
    }
}