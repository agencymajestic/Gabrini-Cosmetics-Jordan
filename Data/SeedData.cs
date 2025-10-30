using GabriniCosmetics.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace GabriniCosmetics.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var context = serviceProvider.GetRequiredService<GabriniCosmeticsContext>();

                string[] roleNames = { "Admin", "User" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                var powerUser = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Gender= "M"
                };

                string userPassword = "Admin@123";
                var user = await userManager.FindByEmailAsync("admin@admin.com");

                if (user == null)
                {
                    var createPowerUser = await userManager.CreateAsync(powerUser, userPassword);
                    if (createPowerUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(powerUser, "Admin");
                    }
                }

                var unknownUser = await userManager.FindByEmailAsync("unknown@GabriniCosmetics.com");
                if (unknownUser == null)
                {
                    unknownUser = new ApplicationUser
                    {
                        UserName = "unknown@GabriniCosmetics.com",
                        Email = "unknown@GabriniCosmetics.com",
                        EmailConfirmed = true,
                        FirstName = "unknown",
                        LastName = "unknown",
                        Gender = "M",
                        LockoutEnabled = true,
                        LockoutEnd = DateTimeOffset.MaxValue,
                    };

                    var createUnknownUser = await userManager.CreateAsync(unknownUser, userPassword);
                    if (createUnknownUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(unknownUser, "User");
                    }
                }

                // Seed Flags
                await SeedFlags(context);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static async Task SeedFlags(GabriniCosmeticsContext context)
        {
            if (!context.Flags.Any())
            {
                var flags = new List<Flag>
            {
                new Flag { Name = "New" },
                new Flag { Name = "Sale" },
                new Flag { Name = "Feature" }
            };

                await context.Flags.AddRangeAsync(flags);
                await context.SaveChangesAsync();
            }
        }
    }

}
