using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace SafePharma.DAL
{
    public static class DALServicesExtention
    {
        public static void AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("GP_TEST");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
                .UseAsyncSeeding(async (context, _, _) =>
                {
                    if (await context.Set<PharmacySettings>().AnyAsync())
                        return;
                    var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                    await context.AddAsync(defaultSettings);
                    await context.SaveChangesAsync();
                })
                .UseSeeding((context, _) =>
                {
                    if (context.Set<PharmacySettings>().Any())
                        return;
                    var defaultSettings = PharmacySettingsSeedingProvider.GetDefaultPharmacySettings();
                    context.Add(defaultSettings);
                    context.SaveChanges();
                })
                );
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPharmacySettingRepository, PharmacySettingRepository>();
        }
    }
}
