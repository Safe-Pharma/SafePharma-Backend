using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PharmacySettingRepository : GenircRepository<PharmacySettings>, IPharmacySettingRepository
    {
        public PharmacySettingRepository(AppDbContext db) : base(db)
        {
        }
        public async Task<PharmacySettings?> GetSettingsByPharmacyId(Guid pharmacyId)
        {
            return await _db.PharmacySettings
                .FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId);
        }
    }
}
