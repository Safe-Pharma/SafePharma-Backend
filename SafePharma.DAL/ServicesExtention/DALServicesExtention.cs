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

                        if (!await context.Set<Audit>().AnyAsync())
                        {
                            var audits = AuditSeeding.GetAudits();
                            await context.AddRangeAsync(audits);
                            await context.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Surface seeding exceptions to the console so they are visible during startup.
                        Console.WriteLine("Error during UseAsyncSeeding: " + ex);
                        throw;
                    }
                })

                .UseSeeding((context, _) =>
                {
                    Console.WriteLine("=== UseSeeding is running ===");
                    try
                    {
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

                        // Identity/User seeding must run via UserManager/RoleManager after the
                        // application service provider is built. Do NOT seed ApplicationUser rows
                        // directly here using the DbContext. Program.cs is responsible for that.

                        if (!context.Set<Audit>().Any())
                        {
                            var audits = AuditSeeding.GetAudits();
                            context.AddRange(audits);
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

            // NOTE: Seeding that requires UserManager/RoleManager
            // should be executed in Program.cs after the application is built.
        }
    }
}