using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SafePharma.DAL
{
    public class OtpRepository : GenircRepository<Otp>, IOtpRepository


    {


        public OtpRepository(AppDbContext db) : base(db)
        {
        }


        public async Task<Otp?> GetValidOtp(Guid customerId, string code)
        {

            var validOtp = await _db.Set<Otp>()
                .Where(o => o.CustomerId==customerId && o.Code==code && !o.IsUsed && o.ExpireDateTime > DateTime.UtcNow).FirstOrDefaultAsync();
                
            return validOtp;




        }


    }
}
