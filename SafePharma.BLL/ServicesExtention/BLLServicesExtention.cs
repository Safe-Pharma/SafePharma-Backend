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

            services.AddScoped<ITaxManager, TaxManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();

            services.AddScoped<IUserLanguageManager, UserLanguageManager>();
        }
    }
}
     
