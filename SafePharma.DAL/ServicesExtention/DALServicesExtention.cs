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
                    Console.WriteLine("=== UseAsyncSeeding is running ===");
                    if (!await context.Set<Country>().AnyAsync())
                    {
                        var countries = CountrySeeding.GetCountries();
                        await context.AddRangeAsync(countries);
                        await context.SaveChangesAsync();
                    }

                    if (await context.Set<ApplicationUser>().AnyAsync())
                        return;
                    var user = UserSeedingProvider.GetUsers();
                    await context.AddRangeAsync(user);
                    await context.SaveChangesAsync();

                    if (await context.Set<PharmacySettings>().AnyAsync())
                        return;
                    var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                    await context.AddAsync(defaultSettings);
                    await context.SaveChangesAsync();

                    if (await context.Set<Audit>().AnyAsync())
                        return;
                    var audit = AuditSeeding.GetAudits();
                    await context.AddRangeAsync(audit);
                    await context.SaveChangesAsync();
                })
                .UseSeeding((context, _) =>
                {
                    Console.WriteLine("=== UseSeeding is running ===");
                    if (!context.Set<Country>().Any())
                    {
                        var countries = CountrySeeding.GetCountries();
                        context.AddRange(countries);
                        context.SaveChanges();
                    }

                    if ( context.Set<ApplicationUser>().Any())
                        return;
                    var user = UserSeedingProvider.GetUsers();
                     context.AddRange(user);
                     context.SaveChanges();

                    if (context.Set<PharmacySettings>().Any())
                        return;
                    var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                    context.Add(defaultSettings);
                    context.SaveChanges();

                    if (context.Set<Audit>().Any())
                        return;
                    var audit = AuditSeeding.GetAudits();
                    context.AddRange(audit);
                    context.SaveChanges();
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

