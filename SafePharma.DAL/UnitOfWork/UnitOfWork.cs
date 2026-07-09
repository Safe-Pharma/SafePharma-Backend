namespace SafePharma.DAL
{
    public class UnitOfWork : IUnitOfWork
    {

        private AppDbContext _db;
        public ITaxRepository TaxRepository { get; }
        public IPharmacySettingRepository PharmacySettingRepository { get; }
        public IAuditRepository _auditRepository { get; }
        public ISubscriptionRepository SubscriptionRepository { get; }
        public IPharmacyRepository PharmacyRepository { get; }
        public IPrimaryContactRepository PrimaryContactRepository { get; }

        public ICountryRepository CountryRepository { get; }

        public ISupplierRepository SupplierRepository { get; }
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



        public UnitOfWork(
            AppDbContext db,
            IAuditRepository auditRepository,
            IPharmacySettingRepository pharmacySettingRepository,
            ISubscriptionRepository subscriptionRepository,
            IPharmacyRepository pharmacyRepository,
            IPrimaryContactRepository primaryContactRepository,
            ITaxRepository taxRepository,
            ICountryRepository countryRepository,
            ISupplierRepository supplierRepository,

            IPaymentVerificationRepository paymentVerificationRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IBatchRepository batchRepository,
            ISupplierPaymentRepository supplierPaymentRepository,
            IPharmacyMedicineRepository pharmacymedicineRepository,
            IMedicineRepository medicineRepository,
            IPurchaseReceiptRepository purchaseReceiptRepository,
            IPurchaseReceiptItemRepository purchaseReceiptItemRepository)


            

        {
            _auditRepository = auditRepository;
            _db = db;
            TaxRepository = taxRepository;
            PharmacySettingRepository = pharmacySettingRepository;
            SubscriptionRepository = subscriptionRepository;
            PharmacyRepository = pharmacyRepository;
            PrimaryContactRepository = primaryContactRepository;
            CountryRepository = countryRepository;
            SupplierRepository = supplierRepository;

            PaymentVerificationRepository = paymentVerificationRepository;
            SubscriptionPlanRepository = subscriptionPlanRepository;
            PaymentMethodRepository = paymentMethodRepository;

            PurchaseOrderRepository = purchaseOrderRepository;
            _batchRepository= batchRepository;
            SupplierPaymentRepository = supplierPaymentRepository;
            PharmacyMedicineRepository= pharmacymedicineRepository;
            MedicineRepository = medicineRepository;
            PurchaseReceiptRepository = purchaseReceiptRepository;
            PurchaseReceiptItemRepository = purchaseReceiptItemRepository;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
