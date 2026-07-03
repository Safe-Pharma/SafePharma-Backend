using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SafePharma.DAL;

namespace SafePharma.BLL
{
    public static class BLLServicesExtention
    {
        public static void AddBLLServices(this IServiceCollection services)
        {
            services.AddScoped<IPharmacySettingManager, PharmacySettingManager>();
            services.AddValidatorsFromAssemblyContaining<PharmacySettingsUpdateDtoValidator>();
            services.AddScoped<IAuditManager, AuditManager>();
<<<<<<< HEAD
            services.AddScoped<ICloudinaryService, CloudinaryService>();
=======
            services.AddScoped<ISubscriptionManager, SubscriptionManager>();
            services.AddScoped<IPasswordHasher<PrimaryContact>, PasswordHasher<PrimaryContact>>();
            services.AddScoped<ITaxManager, TaxManager>();
>>>>>>> main

        }
    }
}
     
