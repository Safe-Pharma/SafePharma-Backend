using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class SupplierRepository : GenircRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> NameExists(Guid pharmacyId, string name, Guid? excludeId = null)
        {
            var query = _db.Set<Supplier>().Where(s =>
                s.PharmacyId == pharmacyId && s.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Supplier>> Search(Guid pharmacyId, string? query)
        {
            var suppliers = _db.Set<Supplier>()
                .AsNoTracking()
                .Include(s => s.Country)
                .Where(s => s.PharmacyId == pharmacyId);

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.ToLower();
                suppliers = suppliers.Where(s =>
                    s.Name.ToLower().Contains(q) ||
                    s.ContactPerson.ToLower().Contains(q) ||
                    s.Email.ToLower().Contains(q) ||
                    s.Phone.ToLower().Contains(q));
            }

            return await suppliers.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetAllForPharmacy(Guid pharmacyId)
        {
            return await _db.Set<Supplier>()
                .AsNoTracking()
                .Include(s => s.Country)
                .Where(s => s.PharmacyId == pharmacyId)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdWithCountry(Guid id)
        {
            return await _db.Set<Supplier>()
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
