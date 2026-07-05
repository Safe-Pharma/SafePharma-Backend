using Microsoft.AspNetCore.Identity;
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

                    // 3. PharmacySettings 
                    if (!await context.Set<PharmacySettings>().AnyAsync())
                    {
                        var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                        await context.AddRangeAsync(defaultSettings);
                        await context.SaveChangesAsync();
                    }
                })
                .UseSeeding((context, _) =>
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

                    // 3. PharmacySettings 
                    if (!context.Set<PharmacySettings>().Any())
                    {
                        var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                        context.AddRange(defaultSettings);
                        context.SaveChanges();
                    }
                })
            );

            services.AddScoped<IPharmacySettingRepository, PharmacySettingRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPharmacyRepository, PharmacyRepository>();
            services.AddScoped<IPrimaryContactRepository, PrimaryContactRepository>();
            services.AddScoped<ITaxRepository, TaxRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICountryRepository, CountryRepository>();
        }
    }
}