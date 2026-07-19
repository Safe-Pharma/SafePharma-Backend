using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class CustomerAllergyRepository : ICustomerAllergyRepository
    {
        private readonly AppDbContext _db;

        public CustomerAllergyRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CustomerAllergy>> GetForCustomer(Guid customerId)
        {
            return await _db.Set<CustomerAllergy>()
                .AsNoTracking()
                .Include(x => x.Allergy)
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<CustomerAllergy?> Find(Guid customerId, Guid allergyId)
        {
            return await _db.Set<CustomerAllergy>()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.AllergyId == allergyId);
        }

        public void Add(CustomerAllergy entity) => _db.Set<CustomerAllergy>().Add(entity);

        public void Remove(CustomerAllergy entity) => _db.Set<CustomerAllergy>().Remove(entity);
    }
}