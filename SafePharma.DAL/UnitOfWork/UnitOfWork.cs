using SafePharma.DAL;
using System;

namespace SafePharma.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPharmacySettingRepository PharmacySettingRepository { get; }

        public ITaxRepository TaxRepository { get; }
        private AppDbContext _db;
        public UnitOfWork(AppDbContext db, IPharmacySettingRepository pharmacySettingRepository , ITaxRepository taxRepository)
        {
            _db = db;
            PharmacySettingRepository = pharmacySettingRepository;
            TaxRepository = taxRepository;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
