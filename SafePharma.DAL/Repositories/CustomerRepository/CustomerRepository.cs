using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class CustomerRepository : GenircRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> PhoneExists(string phone, Guid? excludeId = null)
        {
            var query = _db.Set<Customer>().Where(c => c.Phone == phone);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Customer>> Search(string? query)
        {
            var customers = _db.Set<Customer>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                customers = customers.Where(c =>
                    c.Name.ToLower().Contains(q) ||
                    c.Phone.ToLower().Contains(q) ||
                    (c.Email != null && c.Email.ToLower().Contains(q)));
            }

            return await customers.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Customer?> GetByIdWithHistory(Guid id)
        {
            return await _db.Set<Customer>()
                .Include(c => c.MedicineHistory)
                    .ThenInclude(h => h.Medicine)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByIdWithRealtives(Guid id)
        {
            return await _db.Set<Customer>()
                .Include(c => c.Relatives)
                        .ThenInclude(r => r.Relative)
                        .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
