namespace SafePharma.DAL
{
    public class UnitOfWork : IUnitOfWork
    {

        public ITaxRepository TaxRepository { get; }
        private AppDbContext _db;
        public IPharmacySettingRepository PharmacySettingRepository { get; }
        public IAuditRepository _auditRepository { get; }
        public UnitOfWork(AppDbContext db, IAuditRepository auditRepository, IPharmacySettingRepository pharmacySettingRepository)
        {
            _db = db;
            PharmacySettingRepository = pharmacySettingRepository;
            _auditRepository = auditRepository;
        }
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
