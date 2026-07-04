using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SafePharma.BLL.Managers.users;
using SafePharma.Common;
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
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ISubscriptionManager, SubscriptionManager>();
            services.AddScoped<IPasswordHasher<PrimaryContact>, PasswordHasher<PrimaryContact>>();
<<<<<<< HEAD
            services.AddScoped<ILocationManager, LocationManager>();
=======
            services.AddScoped<ITaxManager, TaxManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
>>>>>>> 80cd4dd169e678d1b3734e80dab4af4c28e06139

            services.AddScoped<IUserLanguageManager, UserLanguageManager>();
        }
    }
}
     
