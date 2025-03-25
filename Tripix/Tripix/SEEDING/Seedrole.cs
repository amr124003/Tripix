using Microsoft.AspNetCore.Identity;

namespace Tripix.SEEDING
{
    public class Seedrole
    {
        public static async Task InitializeAsync ( IServiceProvider serviceProvider )
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "User", "Admin", "SuperAdmin" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }


    }
}
