using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PrimaryContactRepository : GenircRepository<PrimaryContact>, IPrimaryContactRepository
    {
        public PrimaryContactRepository(AppDbContext db) : base(db)
        {
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _db.PrimaryContacts
                .AnyAsync(pc => pc.Email.ToLower() == email.ToLower());
        }

        public async Task<PrimaryContact?> GetByPharmacyId(Guid pharmacyId)
        {
            return await _db.PrimaryContacts
                .AsNoTracking()
                .FirstOrDefaultAsync(pc => pc.PharmacyId == pharmacyId);
        }
    }
}