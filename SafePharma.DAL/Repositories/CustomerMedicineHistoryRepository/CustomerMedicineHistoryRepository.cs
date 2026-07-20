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
        public async Task<CustomerMedicineHistory?> GetByCustomerAndMedicine(Guid customerId, Guid? medicineId)
        {
            return await _db.Set<CustomerMedicineHistory>()
                .FirstOrDefaultAsync(h => h.CustomerId == customerId && h.MedicineId == medicineId);
        }
        public async Task<CustomerMedicineHistory?> FindDuplicate(Guid customerId, Guid? medicineId, string? scientificName)
        {
            if (medicineId.HasValue)
            {
                return await _db.Set<CustomerMedicineHistory>()
                    .FirstOrDefaultAsync(h => h.CustomerId == customerId && h.MedicineId == medicineId.Value);
            }

            if (string.IsNullOrWhiteSpace(scientificName)) return null;

            var normalized = scientificName.Trim().ToLower();
            var manualEntries = await _db.Set<CustomerMedicineHistory>()
                .Where(h => h.CustomerId == customerId && h.MedicineId == null && h.ScientificName != null)
                .ToListAsync();

            return manualEntries.FirstOrDefault(h => h.ScientificName!.Trim().ToLower() == normalized);
        }
    }
}
