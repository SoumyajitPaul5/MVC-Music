using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace MVC_Music.Data
{
    public static class ApplicationDbInitializer
    {
        // Method to seed the database with initial data
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            // Create a new scope for the application services
            ApplicationDbContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                // Apply any pending migrations
                context.Database.Migrate();

                // Creating Roles
                var RoleManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Admin", "Security", "Supervisor", "Staff" };
                IdentityResult roleResult;

                // Loop through each role name and create the role if it does not already exist
                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Creating Users
                var userManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Create Admin user if it does not exist
                if (userManager.FindByEmailAsync("admin@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "admin@outlook.com",
                        Email = "admin@outlook.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Pa55w@rd").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                        userManager.AddToRoleAsync(user, "Security").Wait();
                    }
                }

                // Create Security user if it does not exist
                if (userManager.FindByEmailAsync("security@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "security@outlook.com",
                        Email = "security@outlook.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Pa55w@rd").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Security").Wait();
                    }
                }

                // Create Supervisor user if it does not exist
                if (userManager.FindByEmailAsync("supervisor@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "supervisor@outlook.com",
                        Email = "supervisor@outlook.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Pa55w@rd").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Supervisor").Wait();
                    }
                }

                // Create Staff user if it does not exist
                if (userManager.FindByEmailAsync("staff@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "staff@outlook.com",
                        Email = "staff@outlook.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Pa55w@rd").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Staff").Wait();
                    }
                }

                // Create a general user if it does not exist
                if (userManager.FindByEmailAsync("user@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "user@outlook.com",
                        Email = "user@outlook.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Pa55w@rd").Result;
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the seeding process
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
