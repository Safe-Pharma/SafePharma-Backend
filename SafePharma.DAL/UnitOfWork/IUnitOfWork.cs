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

        public IPaymentVerificationRepository PaymentVerificationRepository { get; }
        public ISubscriptionPlanRepository SubscriptionPlanRepository { get; }
        public IPaymentMethodRepository PaymentMethodRepository { get; }

        public IPurchaseOrderRepository PurchaseOrderRepository { get; }
        public ISupplierPaymentRepository SupplierPaymentRepository { get; }
        public IMedicinePriceRepository MedicinePriceRepository { get; }
        public IMedicineRepository MedicineRepository { get; }
        public IPurchaseReceiptRepository PurchaseReceiptRepository { get; }
        public IPurchaseReceiptItemRepository PurchaseReceiptItemRepository { get; }

        Task SaveAsync();
    }
}