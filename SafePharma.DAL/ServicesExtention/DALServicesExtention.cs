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
                    // ✅ Keep ONLY safe seeding here (no FK dependencies)

                    if (!await context.Set<PharmacySettings>().AnyAsync())
                    {
                        var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                        await context.AddAsync(defaultSettings);
                        await context.SaveChangesAsync();
                    }

                    // ❌ REMOVE Audit from here
                })
                .UseSeeding((context, _) =>
                {
                    if (!context.Set<PharmacySettings>().Any())
                    {
                        var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                        context.Add(defaultSettings);
                        context.SaveChanges();
                    }

                    // ❌ REMOVE Audit from here
                })
            );

            services.AddScoped<IPharmacySettingRepository, PharmacySettingRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPharmacyRepository, PharmacyRepository>();
            services.AddScoped<IPrimaryContactRepository, PrimaryContactRepository>();
            services.AddScoped<ITaxRepository, TaxRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // NOTE: Seeding that requires application services (UserManager/RoleManager)
            // must run after the DI container is fully built (after AddIdentity and app build).
            // Seeding is moved to the application's startup (Program.cs) to ensure
            // Identity services are registered and available.
        }
    }
}