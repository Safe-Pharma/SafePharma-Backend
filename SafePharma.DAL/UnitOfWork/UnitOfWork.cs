namespace SafePharma.DAL
{
    public class UnitOfWork : IUnitOfWork
    {

        public ITaxRepository TaxRepository { get; }
        private AppDbContext _db;
        public IPharmacySettingRepository PharmacySettingRepository { get; }
        public IAuditRepository _auditRepository { get; }
        public ISubscriptionRepository SubscriptionRepository { get; }
        public IPharmacyRepository PharmacyRepository { get; }
        public IPrimaryContactRepository PrimaryContactRepository { get; }
        public ICountryRepository countryRepository { get; }


        public ICountryRepository CountryRepository { get; }

        public UnitOfWork(AppDbContext db, IAuditRepository auditRepository, IPharmacySettingRepository pharmacySettingRepository, ISubscriptionRepository subscriptionRepository,
            IPharmacyRepository pharmacyRepository,ICountryRepository countryRepository,

            IPrimaryContactRepository primaryContactRepository, ITaxRepository taxRepository)


        {
            _db = db;
            PharmacySettingRepository = pharmacySettingRepository;
            _auditRepository = auditRepository;
            SubscriptionRepository = subscriptionRepository;
            PharmacyRepository = pharmacyRepository;
            PrimaryContactRepository = primaryContactRepository;
            TaxRepository = taxRepository;
            CountryRepository = countryRepository;

        }
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
