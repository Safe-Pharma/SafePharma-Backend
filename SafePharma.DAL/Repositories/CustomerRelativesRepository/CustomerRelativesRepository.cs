using SafePharma.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafePharma.DAL 
{
    public class CustomerRelativesRepository : GenircRepository<CustomerRelative>, ICustomerRelativesRepository
    {
        public CustomerRelativesRepository(AppDbContext db) : base(db)
        {
        }
 
    }
}
