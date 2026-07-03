using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PharmacyRepository : GenircRepository<Pharmacy>, IPharmacyRepository
    {
        public PharmacyRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> BusinessEmailExists(string email)
        {
            return await _db.Pharmacies
                .AnyAsync(p => p.BusinessEmail.ToLower() == email.ToLower());
        }
    }
}