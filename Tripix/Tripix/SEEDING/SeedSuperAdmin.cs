#nullable disable
using Microsoft.AspNetCore.Identity;

namespace Tripix.SEEDING
{
    public class SeedSuperAdmin
    {
        public static async Task InitializeAsync ( IServiceProvider serviceProvider )
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string superAdminEmail = Environment.GetEnvironmentVariable("superAdminEmail");
            string superAdminPassword = Environment.GetEnvironmentVariable("superAdminPassword");


            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

            var SuperAdmin = await userManager.FindByEmailAsync(superAdminEmail);

            if (SuperAdmin == null)
            {
                SuperAdmin = new IdentityUser
                {
                    UserName = "SuperAdminv911",
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(SuperAdmin, superAdminPassword);

                if (!result.Succeeded)
                {
                    Console.WriteLine("failed to Add SuperAdmin");
                }
                else
                {
                    await userManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");
                    Console.WriteLine("Super Admin Added Successfuly");
                }

            }
        }
    }
}
