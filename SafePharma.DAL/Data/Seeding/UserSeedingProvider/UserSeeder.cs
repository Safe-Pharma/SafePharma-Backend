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
            // Seed required roles
            var roles = new[] { "admin", "Manager", "assistant", "cashier", "pharmassist", "accountant" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = new ApplicationRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    };

                    var roleResult = await roleManager.CreateAsync(r);
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{role}': {string.Join(", ", roleResult.Errors)}");
                    }
                }
            }

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

            // Ensure admin user is assigned to 'admin' role
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "admin"))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "admin");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to add 'admin' user to role 'admin': {string.Join(", ", addToRoleResult.Errors)}");
                }
            }
        }

        private static List<ApplicationUser> GetUsers()
        {
            // Preserve original seeded users' IDs and add new role users
            var adminId = Guid.Parse("99999999-9999-9999-9999-999999999999");
            var userId = Guid.Parse("88888888-8888-8888-8888-888888888888");
            var managerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var assistantId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var cashierId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            var pharmassistId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
            var accountantId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

            return new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = adminId,
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
                    Id = userId,
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@safepharma.com",
                    NormalizedEmail = "USER@SAFEPHARMA.COM",
                    FirstName = "Normal",
                    LastName = "User",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = managerId,
                    UserName = "Manager",
                    NormalizedUserName = "MANAGER",
                    Email = "manager@safepharma.com",
                    NormalizedEmail = "MANAGER@SAFEPHARMA.COM",
                    FirstName = "Store",
                    LastName = "Manager",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = assistantId,
                    UserName = "assistant",
                    NormalizedUserName = "ASSISTANT",
                    Email = "assistant@safepharma.com",
                    NormalizedEmail = "ASSISTANT@SAFEPHARMA.COM",
                    FirstName = "Pharmacy",
                    LastName = "Assistant",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = cashierId,
                    UserName = "cashier",
                    NormalizedUserName = "CASHIER",
                    Email = "cashier@safepharma.com",
                    NormalizedEmail = "CASHIER@SAFEPHARMA.COM",
                    FirstName = "Store",
                    LastName = "Cashier",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = pharmassistId,
                    UserName = "pharmassist",
                    NormalizedUserName = "PHARMASSIST",
                    Email = "pharmassist@safepharma.com",
                    NormalizedEmail = "PHARMASSIST@SAFEPHARMA.COM",
                    FirstName = "Pharmacy",
                    LastName = "Assistant",
                    EmailConfirmed = true
                },
                new ApplicationUser
                {
                    Id = accountantId,
                    UserName = "accountant",
                    NormalizedUserName = "ACCOUNTANT",
                    Email = "accountant@safepharma.com",
                    NormalizedEmail = "ACCOUNTANT@SAFEPHARMA.COM",
                    FirstName = "Finance",
                    LastName = "Accountant",
                    EmailConfirmed = true
                }
            };
        }
    }
}