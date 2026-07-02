using SafePharma.DAL;
using System;

namespace SafePharma.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPharmacySettingRepository PharmacySettingRepository { get; }


        private AppDbContext _db;
        public UnitOfWork(AppDbContext db, IPharmacySettingRepository pharmacySettingRepository)
        {
            _db = db;
            PharmacySettingRepository = pharmacySettingRepository;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
