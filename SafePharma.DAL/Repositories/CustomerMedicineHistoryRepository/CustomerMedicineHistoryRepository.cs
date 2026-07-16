using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class CustomerMedicineHistoryRepository : GenircRepository<CustomerMedicineHistory>, ICustomerMedicineHistoryRepository
    {
        public CustomerMedicineHistoryRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<CustomerMedicineHistory>> GetForCustomer(Guid customerId, bool? isActive = null)
        {
            var query = _db.Set<CustomerMedicineHistory>()
                .AsNoTracking()
                .Include(h => h.Medicine)
                .Where(h => h.CustomerId == customerId);

            if (isActive.HasValue)
            {
                query = query.Where(h => h.IsActive == isActive.Value);
            }

            return await query.OrderByDescending(h => h.PurchaseDate).ToListAsync();
        }

        public async Task<CustomerMedicineHistory?> GetByIdForCustomer(Guid id, Guid customerId)
        {
            return await _db.Set<CustomerMedicineHistory>()
                .Include(h => h.Medicine)
                .FirstOrDefaultAsync(h => h.Id == id && h.CustomerId == customerId);
        }
    }
}
