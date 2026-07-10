using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SafePharma.BLL;
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
            builder.Services.AddBLLServices();
     
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
            });


            //Register Mail Service
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailSettings"));


            //FrontEnd URL configuration
            builder.Services.Configure<FrontendSettings>(
                builder.Configuration.GetSection("FrontendSettings"));

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
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
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}
