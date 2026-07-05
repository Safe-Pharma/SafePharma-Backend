using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class TaxRepository : GenircRepository<Tax>, ITaxRepository
    {
        public TaxRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> NameExists(Guid pharmacyId, string name, Guid? excludeId = null)
        {
            var query = _db.Taxes.Where(t =>
                t.PharmacyId == pharmacyId && t.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Tax>> Search(Guid pharmacyId, string? query)
        {
            var taxes = _db.Taxes.AsNoTracking().Where(t => t.PharmacyId == pharmacyId);

            if (!string.IsNullOrWhiteSpace(query))
            {
                taxes = taxes.Where(t => t.Name.ToLower().Contains(query.ToLower()));
            }

            return await taxes.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<IEnumerable<Tax>> GetAllForPharmacy(Guid pharmacyId)
        {
            return await _db.Taxes
                .AsNoTracking()
                .Where(t => t.PharmacyId == pharmacyId)
                .ToListAsync();
        }
    }
}