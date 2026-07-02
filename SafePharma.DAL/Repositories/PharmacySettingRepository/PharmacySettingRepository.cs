using Microsoft.EntityFrameworkCore;

namespace SafePharma.DAL
{
    public class PharmacySettingRepository : GenircRepository<PharmacySettings>, IPharmacySettingRepository
    {
        public PharmacySettingRepository(AppDbContext db) : base(db)
        {
        }
        public async Task<PharmacySettings?> GetSettings()
        {
            return await _db.PharmacySettings.FirstOrDefaultAsync();
        }
    }
}
