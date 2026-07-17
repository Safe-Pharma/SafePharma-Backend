using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafePharma.DAL.Data.Seeding.PaymentMethodSeedingProvider;
using SafePharma.DAL.Data.Seeding.SubscriptionPlanSeedingProvider;
using SafePharma.DAL.Data.Seeding.UserSeedingProvider;
namespace SafePharma.DAL
{
    public static class DALServicesExtention
    {
        public static void AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
                .UseAsyncSeeding(SeedAsync)
                .UseSeeding(Seed)
            );


            services.AddScoped<IPharmacySettingRepository, PharmacySettingRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPharmacyRepository, PharmacyRepository>();
            services.AddScoped<IPrimaryContactRepository, PrimaryContactRepository>();
            services.AddScoped<ITaxRepository, TaxRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerMedicineHistoryRepository, CustomerMedicineHistoryRepository>();
            services.AddScoped<ISupplierPaymentRepository, SupplierPaymentRepository>();
            services.AddScoped<IPaymentVerificationRepository, PaymentVerificationRepository>();
            services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IMedicineRepository, MedicineRepository>();
            services.AddScoped<IPharmacyMedicineRepository, PharmacyMedicineRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IPurchaseReceiptRepository, PurchaseReceiptRepository>();
            services.AddScoped<IPurchaseReceiptItemRepository, PurchaseReceiptItemRepository>();
            services.AddScoped<IBatchRepository, BatchRepository>();
            services.AddScoped<IManufacturerBarcodeRepository, ManufacturerBarcodeRepository>();
            services.AddScoped<IPharmacyBarcodeRepository, PharmacyBarcodeRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<IGenircRepository<Allergy>, GenircRepository<Allergy>>();
            services.AddScoped<IGenircRepository<ChronicCondition>, GenircRepository<ChronicCondition>>();


            services.AddScoped<IUnitOfWork, UnitOfWork>();


        }
        private static async Task SeedAsync(DbContext context, bool _, CancellationToken __)
        {

            try
            {
                // 1. Countries
                if (!await context.Set<Country>().AnyAsync())
                {
                    var countries = CountrySeeding.GetCountries();
                    await context.AddRangeAsync(countries);
                    await context.SaveChangesAsync();
                }

                // 2. Subscriptions + Pharmacies
                if (!await context.Set<Pharmacy>().AnyAsync())
                {
                    var subscriptions = PharmacySeeding.GetSubscriptionsWithPharmacies();
                    await context.AddRangeAsync(subscriptions);
                    await context.SaveChangesAsync();
                }

                // 3. PharmacySettings (بعد الـ Pharmacies)
                if (!await context.Set<PharmacySettings>().AnyAsync())
                {
                    var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                    await context.AddRangeAsync(defaultSettings);
                    await context.SaveChangesAsync();
                }

                // 4. Audit
                //if (!await context.Set<ApplicationUser>().AnyAsync())
                //{
                //    var audits = UserSeeder.GetUsers();
                //    await context.AddRangeAsync(audits);
                //    await context.SaveChangesAsync();
                //}

                //if (!await context.Set<Audit>().AnyAsync())
                //{
                //    var audits = AuditSeeding.GetAudits();
                //    await context.AddRangeAsync(audits);
                //    await context.SaveChangesAsync();
                //}
                // 5. Taxes 
                if (!await context.Set<Tax>().AnyAsync())
                {
                    var taxes = TaxSeedingProvider.GetTaxes();
                    await context.AddRangeAsync(taxes);
                    await context.SaveChangesAsync();
                }

                // 6. Medicines (global catalog)
                if (!await context.Set<Medicine>().AnyAsync())
                {
                    var medicines = MedicineSeedingProvider.GetMedicines();
                    await context.AddRangeAsync(medicines);
                    await context.SaveChangesAsync();
                }

                // 6b. Medicine Prices (needs Medicines and Taxes to already be seeded, per pharmacy)
                if (!await context.Set<PharmacyMedicine>().AnyAsync())
                {
                    var medicines = await context.Set<Medicine>().ToListAsync();
                    var taxes = await context.Set<Tax>().ToListAsync();
                    var pharmacyMedicines = PharmacyMedicineSeedingProvider.GetPharmacyMedicines(medicines, taxes);
                    await context.AddRangeAsync(pharmacyMedicines);
                    await context.SaveChangesAsync();
                }
                // 7. Suppliers
                if (!await context.Set<Supplier>().AnyAsync())
                {
                    var suppliers = SupplierSeeding.GetSuppliers();
                    await context.AddRangeAsync(suppliers);
                    await context.SaveChangesAsync();
                }
                //// 8. Supplier Payments
                //if (!await context.Set<SupplierPayment>().AnyAsync())
                //{
                //    var payments = SupplierPaymentSeeding.GetPayments();
                //    await context.AddRangeAsync(payments);
                //    await context.SaveChangesAsync();
                //}

                // 9. Subscription Plans
                if (!await context.Set<SubscriptionPlan>().AnyAsync())
                {
                    await context.AddRangeAsync(SubscriptionPlanSeeding.GetPlans());
                    await context.SaveChangesAsync();
                }
                // 10. Payment Methods
                if (!await context.Set<PaymentMethod>().AnyAsync())
                {
                    await context.AddRangeAsync(PaymentMethodSeeding.GetMethods());
                    await context.SaveChangesAsync();
                }
                // 11. Purchase Orders (needs Suppliers to already be seeded)
                if (!await context.Set<PurchaseOrder>().AnyAsync())
                {
                    var suppliers = await context.Set<Supplier>().ToListAsync();
                    var purchaseOrders = PurchaseOrderSeeding.GetPurchaseOrders(suppliers);
                    await context.AddRangeAsync(purchaseOrders);
                    await context.SaveChangesAsync();
                }
                // 12. Purchase Order Items (needs Medicines and PurchaseOrders to already be seeded)
                if (!await context.Set<PurchaseOrderItem>().AnyAsync())
                {
                    var pharmacyMedicines = await context.Set<PharmacyMedicine>()
                        .Include(pm => pm.Medicine)
                        .ToListAsync();

                    var purchaseOrderItems = PurchaseOrderItemSeeding
                        .GetPurchaseOrderItems(pharmacyMedicines);

                    await context.AddRangeAsync(purchaseOrderItems);

                    await context.SaveChangesAsync();
                }

                //------------
                if (!await context.Set<Allergy>().AnyAsync())
                {
                    await context.AddRangeAsync(AllergySeeding.GetAllergies());
                    await context.SaveChangesAsync();
                }

                if (!await context.Set<ChronicCondition>().AnyAsync())
                {
                    await context.AddRangeAsync(ChronicConditionSeeding.GetConditions());
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during UseAsyncSeeding: " + ex);
                throw;
            }
        }

        private static void Seed(DbContext context, bool _)
        {
            Console.WriteLine("=== UseSeeding is running ===");
            try
            {
                // 1. Countries
                if (!context.Set<Country>().Any())
                {
                    var countries = CountrySeeding.GetCountries();
                    context.AddRange(countries);
                    context.SaveChanges();
                }

                // 2. Subscriptions + Pharmacies
                if (!context.Set<Pharmacy>().Any())
                {
                    var subscriptions = PharmacySeeding.GetSubscriptionsWithPharmacies();
                    context.AddRange(subscriptions);
                    context.SaveChanges();
                }

                // 3. PharmacySettings (بعد الـ Pharmacies)
                if (!context.Set<PharmacySettings>().Any())
                {
                    var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                    context.AddRange(defaultSettings);
                    context.SaveChanges();
                }

                // 4. Audit
                //if (!context.Set<ApplicationUser>().Any())
                //{
                //    var users = UserSeeder.GetUsers();
                //    context.AddRange(users);
                //    context.SaveChanges();
                //}
                //if (!context.Set<Audit>().Any())
                //{
                //    var audits = AuditSeeding.GetAudits();
                //    context.AddRange(audits);
                //    context.SaveChanges();
                //}
                // 5. Taxes
                if (!context.Set<Tax>().Any())
                {
                    var taxes = TaxSeedingProvider.GetTaxes();
                    context.AddRange(taxes);
                    context.SaveChanges();
                }

                // 6. Medicines (global catalog)
                if (!context.Set<Medicine>().Any())
                {
                    var medicines = MedicineSeedingProvider.GetMedicines();
                    context.AddRange(medicines);
                    context.SaveChanges();
                }

                // 6b. Medicine Prices (needs Medicines and Taxes to already be seeded, per pharmacy)
                if (!context.Set<PharmacyMedicine>().Any())
                {
                    var medicines = context.Set<Medicine>().ToList();
                    var taxes = context.Set<Tax>().ToList();
                    var pharmacyMedicines = PharmacyMedicineSeedingProvider.GetPharmacyMedicines(medicines, taxes);
                    context.AddRange(pharmacyMedicines);
                    context.SaveChanges();
                }
                // 7. Suppliers 
                if (!context.Set<Supplier>().Any())
                {
                    var suppliers = SupplierSeeding.GetSuppliers();
                    context.AddRange(suppliers);
                    context.SaveChanges();
                }
                //// 8. Supplier Payments
                //if (!context.Set<SupplierPayment>().Any())
                //{
                //    var payments = SupplierPaymentSeeding.GetPayments();
                //    context.AddRange(payments);
                //    context.SaveChanges();
                //}
                // 9. Subscription Plans
                if (!context.Set<SubscriptionPlan>().Any())
                {
                    context.AddRange(SubscriptionPlanSeeding.GetPlans());
                    context.SaveChanges();
                }
                // 10. Payment Methods
                if (!context.Set<PaymentMethod>().Any())
                {
                    context.AddRange(PaymentMethodSeeding.GetMethods());
                    context.SaveChanges();
                }
                // 11. Purchase Orders (needs Suppliers to already be seeded)
                if (!context.Set<PurchaseOrder>().Any())
                {
                    var suppliers = context.Set<Supplier>().ToList();
                    var purchaseOrders = PurchaseOrderSeeding.GetPurchaseOrders(suppliers);
                    context.AddRange(purchaseOrders);
                    context.SaveChanges();
                }
                // 12. Purchase Order Items (needs Medicines and PurchaseOrders to already be seeded)
                if (!context.Set<PurchaseOrderItem>().Any())
                {
                    var pharmacyMedicines = context.Set<PharmacyMedicine>()
                        .Include(pm => pm.Medicine)
                        .ToList();

                    var purchaseOrderItems = PurchaseOrderItemSeeding
                        .GetPurchaseOrderItems(pharmacyMedicines);

                    context.AddRange(purchaseOrderItems);

                    context.SaveChanges();
                }




                // ------------
                if (!context.Set<Allergy>().Any())
                {
                    context.AddRange(AllergySeeding.GetAllergies());
                    context.SaveChanges();
                }

                if (!context.Set<ChronicCondition>().Any())
                {
                    context.AddRange(ChronicConditionSeeding.GetConditions());
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during UseSeeding: " + ex);
                throw;
            }
        }


        public static async Task UseDALSeedingAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                var context = services.GetRequiredService<AppDbContext>();

                await UserSeeder.SeedAsync(userManager, roleManager);

                if (!context.Set<Audit>().Any())
                {
                    var audit = AuditSeeding.GetAudits();
                    context.AddRange(audit);
                    context.SaveChanges();
                }
                Console.WriteLine("=== Identity & Audit seeding completed successfully ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during UseDALSeedingAsync: " + ex);
                throw;
            }


        }
    }
}