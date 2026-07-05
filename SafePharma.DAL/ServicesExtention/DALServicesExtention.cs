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
                    if (!await context.Set<PharmacySettings>().AnyAsync())
                    {
                        var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                        await context.AddAsync(defaultSettings);
                        await context.SaveChangesAsync();
                    }

                    if (!await context.Set<Country>().AnyAsync())
                    {
                        var countries = CountrySeeding.GetCountries();
                        await context.AddRangeAsync(countries);
                        await context.SaveChangesAsync();
                    }

                    if (!await context.Set<Pharmacy>().AnyAsync())
                    {
                        var subscriptions = PharmacySeeding.GetSubscriptionsWithPharmacies();
                        await context.AddRangeAsync(subscriptions);
                        await context.SaveChangesAsync();
                    }

                    if (!await context.Set<ApplicationUser>().AnyAsync())
                    {
                        var users = UserSeedingProvider.GetUsers();
                        await context.AddRangeAsync(users);
                        await context.SaveChangesAsync();
                    }

                    if (!await context.Set<Audit>().AnyAsync())
                    {
                        var audits = AuditSeeding.GetAudits();
                        await context.AddRangeAsync(audits);
                        await context.SaveChangesAsync();
                    }
                })

                .UseSeeding((context, _) =>
                {
                    Console.WriteLine("=== UseSeeding is running ===");

                    if (!context.Set<PharmacySettings>().Any())
                    {
                        var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                        context.Add(defaultSettings);
                        context.SaveChanges();
                    }

                    if (!context.Set<Country>().Any())
                    {
                        var countries = CountrySeeding.GetCountries();
                        context.AddRange(countries);
                        context.SaveChanges();
                    }

                    if (!context.Set<Pharmacy>().Any())
                    {
                        var subscriptions = PharmacySeeding.GetSubscriptionsWithPharmacies();
                        context.AddRange(subscriptions);
                        context.SaveChanges();
                    }

                    if (!context.Set<ApplicationUser>().Any())
                    {
                        var users = UserSeedingProvider.GetUsers();
                        context.AddRange(users);
                        context.SaveChanges();
                    }

                    if (!context.Set<Audit>().Any())
                    {
                        var audits = AuditSeeding.GetAudits();
                        context.AddRange(audits);
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
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // NOTE: Seeding that requires UserManager/RoleManager
            // should be executed in Program.cs after the application is built.
        }
    }
}