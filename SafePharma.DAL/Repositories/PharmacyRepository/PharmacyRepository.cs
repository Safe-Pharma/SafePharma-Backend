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
        public async Task<bool> TaxNumberExists(string taxNumber)
        {
            return await _db.Pharmacies
                .AnyAsync(p => p.TaxNumber == taxNumber);
        }

        public async Task<bool> CommercialRegistrationExists(string commercialRegistration)
        {
            return await _db.Pharmacies
                .AnyAsync(p => p.CommercialRegistration == commercialRegistration);
        }
    }
}