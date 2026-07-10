using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SafePharma.BLL.Managers;
using SafePharma.BLL.Managers.AuthenticationManager;
using SafePharma.BLL.Managers.users;
using SafePharma.BLL.Validators.PaymentValidator;
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
            services.AddScoped<ISupplierManager, SupplierManager>();
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IPurchaseOrderManager, PurchaseOrderManager>();
            services.AddValidatorsFromAssemblyContaining<LoginValidator>();
            services.AddValidatorsFromAssemblyContaining<ChangePasswordValidator>();
            services.AddValidatorsFromAssemblyContaining<TaxCreateDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<TaxUpdateDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<SupplierCreateDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<SupplierUpdateDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<PurchaseOrderCreateDtoValidator>();
            services.AddScoped<ISupplierPaymentManager, SupplierPaymentManager>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICurrentUserContext, HttpCurrentUserContext>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<IUserLanguageManager, UserLanguageManager>();
            services.AddScoped<ILocationManager, LocationManager>();
            services.AddScoped<IPaymentManager, PaymentManager>();
            services.AddScoped<ISubscriptionPlanManager, SubscriptionPlanManager>();
            services.AddScoped<IPaymentMethodManager, PaymentMethodManager>();
            services.AddScoped<IPurchaseReceiptManager, PurchaseReceiptManager>();
            services.AddValidatorsFromAssemblyContaining<SubscriptionPlanUpsertDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<PaymentMethodUpsertDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<RecordSupplierPaymentDtoValidator>();

            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateUserValidator>();

            services.AddScoped<IMedicineManager, MedicineManager>();
            services.AddValidatorsFromAssemblyContaining<MedicineCreateDtoValidator>();

            services.AddValidatorsFromAssemblyContaining<PharmacyMedicineUpdateDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<GlobalMedicineUpdateDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<LinkExistingMedicineDtoValidator>();



            services.AddHttpClient<IEmailService, EmailService>();


        }
    }
}

