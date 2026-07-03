namespace SafePharma.DAL
{
    public interface IUnitOfWork
    {

        public IPharmacySettingRepository PharmacySettingRepository { get; }
        public IAuditRepository _auditRepository { get; }
        public ISubscriptionRepository SubscriptionRepository { get; }
        public IPharmacyRepository PharmacyRepository { get; }
        public IPrimaryContactRepository PrimaryContactRepository { get; }
        Task SaveAsync();
    }
}