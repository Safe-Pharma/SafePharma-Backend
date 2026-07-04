using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SafePharma.DAL.Data.Seeding.UserSeedingProvider
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            var users = GetUsers();

            foreach (var user in users)
            {
                var existing = await userManager.FindByNameAsync(user.UserName);

                string password = user.UserName switch
                {
                    "admin" => "Admin@123",
                    "user" => "User@123",
                    _ => "Default@123"
                };

                if (existing == null)
                {
                    // Create new user with password
                    var result = await userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user {user.UserName}: " +
                            string.Join(", ", result.Errors));
                    }
                }
                else
                {
                    // 🔁 Reset password for existing user
                    var token = await userManager.GeneratePasswordResetTokenAsync(existing);
                    var resetResult = await userManager.ResetPasswordAsync(existing, token, password);

                    if (!resetResult.Succeeded)
                    {
                        throw new Exception($"Failed to reset password for {existing.UserName}: " +
                            string.Join(", ", resetResult.Errors));
                    }
                }
            }
        }

        private static List<ApplicationUser> GetUsers()
        {
            var u1 = Guid.Parse("99999999-9999-9999-9999-999999999999");
            var u2 = Guid.Parse("88888888-8888-8888-8888-888888888888");

            return new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = u1,
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@safepharma.com",
                    NormalizedEmail = "ADMIN@SAFEPHARMA.COM",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = u2,
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@safepharma.com",
                    NormalizedEmail = "USER@SAFEPHARMA.COM",
                    FirstName = "Normal",
                    LastName = "User",
                    EmailConfirmed = true
                }
            };
        }
    }
}