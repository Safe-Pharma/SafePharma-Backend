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
        public ICustomerRepository CustomerRepository { get; }
        public ICustomerPharmacyBalanceRepository CustomerPharmacyBalanceRepository { get; }
        public ICustomerMedicineHistoryRepository CustomerMedicineHistoryRepository { get; }

        public IPaymentVerificationRepository PaymentVerificationRepository { get; }
        public ISubscriptionPlanRepository SubscriptionPlanRepository { get; }
        public IPaymentMethodRepository PaymentMethodRepository { get; }

        public IPurchaseOrderRepository PurchaseOrderRepository { get; }
        public IBatchRepository _batchRepository { get; }

        public ISupplierPaymentRepository SupplierPaymentRepository { get; }
        public IPharmacyMedicineRepository PharmacyMedicineRepository { get; }
        public IMedicineRepository MedicineRepository { get; }
        public IPurchaseReceiptRepository PurchaseReceiptRepository { get; }
        public IPurchaseReceiptItemRepository PurchaseReceiptItemRepository { get; }
        public ISaleRepository SaleRepository { get; }


        IManufacturerBarcodeRepository ManufacturerBarcodeRepository { get; }
        IPharmacyBarcodeRepository PharmacyBarcodeRepository { get; }
        public IGenircRepository<Allergy> AllergyRepository { get; }
        public IGenircRepository<ChronicCondition> ChronicConditionRepository { get; }
        public IGenircRepository<Organ> OrganRepository { get; }

        public IGenircRepository<OrganImpairmentLevel> OrganImpairmentLevelRepository { get; }
        public INotificationRepository Notifications { get; }

        Task SaveAsync();
    }
}