namespace SafePharma.DAL
{
    public interface IUnitOfWork
    {

        public IPharmacySettingRepository PharmacySettingRepository { get; }
        public IAuditRepository _auditRepository { get; }
        public ISubscriptionRepository SubscriptionRepository { get; }
        public IPharmacyRepository PharmacyRepository { get; }
        public IPrimaryContactRepository PrimaryContactRepository { get; }
<<<<<<< HEAD
        ICountryRepository CountryRepository { get; }
=======
        public ITaxRepository TaxRepository { get; }
>>>>>>> 80cd4dd169e678d1b3734e80dab4af4c28e06139
        Task SaveAsync();
    }
}