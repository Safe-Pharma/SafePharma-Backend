using Microsoft.EntityFrameworkCore;
using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL 
{
    public class CustomerRelativesRepository : GenircRepository<CustomerRelative>, ICustomerRelativesRepository
    {
        private readonly AppDbContext d;

        public CustomerRelativesRepository(AppDbContext db) : base(db)
        {
        }
        public async Task<bool> HasPortalAccessAsync(Guid requesterId, Guid targetCustomerId)
        {
            return await _db.Set<CustomerRelative>()
                .AnyAsync(cr =>
                    cr.CustomerId == requesterId &&
                    cr.RelativeId == targetCustomerId &&
                    cr.HasAccessToRelative==true);
        }
    }
}
