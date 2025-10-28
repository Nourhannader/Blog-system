using Microsoft.AspNetCore.Identity;

namespace Blog.Data
{
    public class DatabaseSeeder
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DatabaseSeeder(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task SeedAsync()
        {
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // Seed admin user
            if (await userManager.FindByEmailAsync("nourhannaderkhattab@gmail.com") == null)
            {
                var adminUser = new AppUser {UserName = "talaat", Email = "nourhannaderkhattab@gmail.com" };
                await userManager.CreateAsync(adminUser, "Talaat@12345");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed regular user
            if (await userManager.FindByEmailAsync("nourhannader425@gmail.com") == null)
            {
                var regularUser = new AppUser { UserName = "Nour", Email = "nourhannader425@gmail.com" };
                await userManager.CreateAsync(regularUser, "Nour@12345");
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }
}
