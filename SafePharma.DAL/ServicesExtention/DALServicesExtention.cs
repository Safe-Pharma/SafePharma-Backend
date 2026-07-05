using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace SafePharma.DAL
{
    public static class DALServicesExtention
    {
        public static void AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
                .UseAsyncSeeding(async (context, _, _) =>
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

                        //// 4. Audit
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during UseAsyncSeeding: " + ex);
                        throw;
                    }
                })
                .UseSeeding((context, _) =>
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

                        //// 4. Audit
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during UseSeeding: " + ex);
                        throw;
                    }
                })
            );

            services.AddScoped<IPharmacySettingRepository, PharmacySettingRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPharmacyRepository, PharmacyRepository>();
            services.AddScoped<IPrimaryContactRepository, PrimaryContactRepository>();
            services.AddScoped<ITaxRepository, TaxRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}