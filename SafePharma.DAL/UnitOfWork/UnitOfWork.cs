using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        public IManufacturerBarcodeRepository ManufacturerBarcodeRepository { get; }
        public IPharmacyBarcodeRepository PharmacyBarcodeRepository { get; }
        public ISaleRepository SaleRepository { get; }

        public IGenircRepository<Allergy> AllergyRepository { get; }
        public IGenircRepository<ChronicCondition> ChronicConditionRepository { get; }
        public IGenircRepository<Organ> OrganRepository { get; }

        public IGenircRepository<OrganImpairmentLevel> OrganImpairmentLevelRepository { get; }
        public INotificationRepository Notifications { get; }

        public ICustomerAllergyRepository CustomerAllergyRepository { get; } 

        public ICustomerChronicConditionRepository CustomerChronicConditionRepository { get; }
        public ICustomerOrganFunctionRepository CustomerOrganFunctionRepository { get; }
        public ICustomerRelativesRepository _customerRelativesRepository { get; }


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
            ICustomerRepository customerRepository,
            ICustomerPharmacyBalanceRepository customerPharmacyBalanceRepository,
            ICustomerMedicineHistoryRepository customerMedicineHistoryRepository,

            IPaymentVerificationRepository paymentVerificationRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IBatchRepository batchRepository,
            ISupplierPaymentRepository supplierPaymentRepository,
            IPharmacyMedicineRepository pharmacymedicineRepository,
            IMedicineRepository medicineRepository,
            IPurchaseReceiptRepository purchaseReceiptRepository,
            IPurchaseReceiptItemRepository purchaseReceiptItemRepository,
            IManufacturerBarcodeRepository manufacturerBarcodeRepository,
            IPharmacyBarcodeRepository pharmacyBarcodeRepository,
            IGenircRepository<Allergy> allergyRepository,
            IGenircRepository<ChronicCondition> chronicConditionRepository,
            ISaleRepository saleRepository,
            IGenircRepository<Organ> organRepository,
            IGenircRepository<OrganImpairmentLevel> organImpairmentLevelRepository,
            INotificationRepository notificationRepository,
            ICustomerAllergyRepository customerAllergyRepository,
            ICustomerChronicConditionRepository customerChronicConditionRepository,
            ICustomerOrganFunctionRepository customerOrganFunctionRepository
,
            ICustomerRelativesRepository customerRelativesRepository


            )




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
            CustomerRepository = customerRepository;
            CustomerPharmacyBalanceRepository = customerPharmacyBalanceRepository;
            CustomerMedicineHistoryRepository = customerMedicineHistoryRepository;

            PaymentVerificationRepository = paymentVerificationRepository;
            SubscriptionPlanRepository = subscriptionPlanRepository;
            PaymentMethodRepository = paymentMethodRepository;

            PurchaseOrderRepository = purchaseOrderRepository;
            _batchRepository = batchRepository;
            SupplierPaymentRepository = supplierPaymentRepository;
            PharmacyMedicineRepository = pharmacymedicineRepository;
            MedicineRepository = medicineRepository;
            PurchaseReceiptRepository = purchaseReceiptRepository;
            PurchaseReceiptItemRepository = purchaseReceiptItemRepository;
            ManufacturerBarcodeRepository = manufacturerBarcodeRepository;
            PharmacyBarcodeRepository = pharmacyBarcodeRepository;
            AllergyRepository = allergyRepository;
            ChronicConditionRepository = chronicConditionRepository;
            SaleRepository = saleRepository;
            OrganRepository = organRepository;
            OrganImpairmentLevelRepository = organImpairmentLevelRepository;
            Notifications = notificationRepository;
            CustomerAllergyRepository = customerAllergyRepository;
            CustomerChronicConditionRepository = customerChronicConditionRepository;
            CustomerOrganFunctionRepository = customerOrganFunctionRepository;
            _customerRelativesRepository = customerRelativesRepository;
        }

        public async Task SaveAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
                                                (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                if (sqlEx.Message.Contains("IX_PharmacyMedicine_Pharmacy_SKU", StringComparison.OrdinalIgnoreCase))
                {
                    throw new DuplicateSkuException(ex);
                }
                throw;
            }
        }
        public void ClearTracking()
        {
            _db.ChangeTracker.Clear();
        }
    }
}