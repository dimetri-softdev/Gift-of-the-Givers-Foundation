using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            var roles = new[] { "Admin", "Employee", "Donor", "Volunteer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            var adminEmail = "admin@giftofthegivers.org";
            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Site Administrator",
                    Role = "Admin"
                };

                var createResult = await userManager.CreateAsync(admin, "Admin123!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(admin, "Admin"))
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

        public static async Task SeedReliefProjectsAsync(ApplicationDbContext context)
        {
            if (await context.ReliefProjects.AnyAsync())
            {
                return;
            }

            var projects = new[]
            {
                new ReliefProject
                {
                    Title = "Emergency Food Aid",
                    Location = "Western Cape, South Africa",
                    TargetAmount = 10000m,
                    RaisedAmount = 0m,
                    Status = "Active"
                },
                new ReliefProject
                {
                    Title = "Flood Relief Supplies",
                    Location = "KwaZulu-Natal, South Africa",
                    TargetAmount = 20000m,
                    RaisedAmount = 0m,
                    Status = "Active"
                },
                new ReliefProject
                {
                    Title = "Medical Outreach",
                    Location = "Eastern Cape, South Africa",
                    TargetAmount = 15000m,
                    RaisedAmount = 0m,
                    Status = "Pending"
                }
            };

            await context.ReliefProjects.AddRangeAsync(projects);
            await context.SaveChangesAsync();
        }
    }
}