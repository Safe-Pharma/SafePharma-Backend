namespace SafePharma.DAL
{
    public interface IUnitOfWork
    {

        public IPharmacySettingRepository PharmacySettingRepository { get; }
        public IAuditRepository _auditRepository { get; }
        public ISubscriptionRepository SubscriptionRepository { get; }
        public IPharmacyRepository PharmacyRepository { get; }
        public IPrimaryContactRepository PrimaryContactRepository { get; }
        public ITaxRepository TaxRepository { get; }
        ICountryRepository CountryRepository { get; }
        public ISupplierRepository SupplierRepository { get; }
        public IPurchaseOrderRepository PurchaseOrderRepository { get; }

        Task SaveAsync();
    }
}