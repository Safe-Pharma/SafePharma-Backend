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
        public async Task <IEnumerable<Audit>>GetAuditsWithUsers()
        {
            return _db.Set<Audit>().Include(a => a.User);
        }
        public async Task<Audit> GetAuditWithUserId(Guid id)
        {
            return _db.Set<Audit>().Include(a => a.User).FirstOrDefault(u => u.UserId == id)!;
        }


    }
}
