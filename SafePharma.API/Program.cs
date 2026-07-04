using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafePharma.BLL;
using SafePharma.DAL;
using Scalar.AspNetCore;
using System.Text;
namespace SafePharma.API

{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });
            builder.Services.AddOpenApi();
            builder.Services.AddDALServices(builder.Configuration);
            builder.Services.AddBLLServices();
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                            .AddEntityFrameworkStores<AppDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                    )
                };
            });

<<<<<<< HEAD
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


=======
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
>>>>>>> 80cd4dd169e678d1b3734e80dab4af4c28e06139

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
<<<<<<< HEAD
            app.UseCors("AllowAngularDev");
=======
            app.UseCors("AllowAll");
>>>>>>> 80cd4dd169e678d1b3734e80dab4af4c28e06139
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}
