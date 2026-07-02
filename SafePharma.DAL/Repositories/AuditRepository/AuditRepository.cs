using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL
{
    public class AuditRepository : GenircRepository<Audit>
    {
        public AuditRepository(AppDbContext db) : base(db)
        {
        }
        

    }
}
