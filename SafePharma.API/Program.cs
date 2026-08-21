using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SafePharma.AI;
using SafePharma.AI.Rag;
using SafePharma.BLL;
using SafePharma.BLL.BackgroundJobs;
using SafePharma.Common;
using SafePharma.DAL;
using Scalar.AspNetCore;
using System.Text;
namespace SafePharma.API

{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });
            builder.Services.AddOpenApi();
            // Bind JwtSettings from configuration
            builder.Services.Configure<SafePharma.Common.JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.AddDALServices(builder.Configuration);
            builder.Services.AddBLLServices(builder.Configuration);
            builder.Services.AddAIServices(builder.Configuration);

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                     .AddEntityFrameworkStores<AppDbContext>()
                     .AddDefaultTokenProviders();
            // Authentication configuration - use JWT Bearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // for local development allow non-https tokens (adjust for production)
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],

                    // Ensure the key is present to avoid ArgumentNullException in Encoding.GetBytes
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key is not configured. Add 'JWT:Key' to appsettings.json."))
                    ),

                    // Ensure role and name claims are mapped correctly
                    NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,

                    // reduce clock skew for more deterministic tests
                    ClockSkew = System.TimeSpan.FromMinutes(1)
                };
            });

            // Register authorization services
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(SafePharma.BLL.AuthPolicies.OwnerOnly, policy =>
                    policy.RequireRole("Owner"));
                options.AddPolicy(SafePharma.BLL.AuthPolicies.AdminOrOwner, policy =>
                    policy.RequireRole("admin", "Owner"));
            });


            //Register Mail Service
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));


            //FrontEnd URL configuration
            builder.Services.Configure<FrontendSettings>(
                builder.Configuration.GetSection("FrontendSettings"));

            // CORS
            // Restricted to the actual deployed frontend + local dev server.
            // Was AllowAnyOrigin() while we didn't have a real frontend URL yet —
            // now that we do, lock it down. The policy is still called "AllowAll"
            // only because app.UseCors("AllowAll") already references that name.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(
                        "https://safepharma-app-cfbnewd5efhabvdz.switzerlandnorth-01.azurewebsites.net",
                        "http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            //Hangfire configuration
            builder.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddHangfireServer();


            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Run data seeding that depends on Identity and the final service provider.
            try
            {
                await app.Services.UseDALSeedingAsync();
            }
            catch
            {
                // ignore seeding errors at startup
            }

            // Configure the HTTP request pipeline.
            // API docs (Scalar/OpenAPI) are always on in Development, and in any
            // other environment (e.g. this App Service's Production) only when
            // explicitly turned on via the "EnableApiDocs" setting — so it can be
            // toggled from Azure's Environment variables without a redeploy, but
            // stays off by default so the API surface isn't publicly browsable.
            var enableApiDocs = app.Environment.IsDevelopment() ||
                builder.Configuration.GetValue<bool>("EnableApiDocs");

            if (enableApiDocs)
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire");
            //RecurringJob.AddOrUpdate<IExpiryNotificationJob>(
            //    "expiry-notification-job",
            //    job => job.Execute(),
            //    Cron.Daily);

            RecurringJob.AddOrUpdate<IExpiryNotificationJob>(
                "expiry-notification-job",
                job => job.Execute(),
                Cron.Minutely);

            app.MapControllers();
            app.Run();
        }
    }
    ////////////
}