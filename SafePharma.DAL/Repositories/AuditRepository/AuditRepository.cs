using Microsoft.EntityFrameworkCore;
using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL
{
    public class AuditRepository : GenircRepository<Audit>, IAuditRepository
    {
        public AuditRepository(AppDbContext db) : base(db)
        {
        }
        public async Task<IEnumerable<Audit>> GetAuditsWithUsers(Guid pharmacyId)
        {
            return await _db.Set<Audit>().Where(a => a.PharmacyId == pharmacyId).Include(a => a.User).ToListAsync();
        }

        public async Task<Audit?> GetAuditWithUserId(Guid id)
        {
            return await _db.Set<Audit>().Include(a => a.User).FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<IEnumerable<Audit>> GetRecentForPharmacy(Guid pharmacyId, int take)
        {
            return await _db.Set<Audit>()
                .Include(a => a.User)
                .Where(a => a.PharmacyId == pharmacyId)
                .OrderByDescending(a => a.Date)
                .Take(take)
                .ToListAsync();
        }
    }
}
