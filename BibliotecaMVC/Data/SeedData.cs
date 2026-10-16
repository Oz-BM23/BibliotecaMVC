using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var config = services.GetRequiredService<IConfiguration>();

            string[] roles = new[] { "Admin", "Bibliotecario", "Usuario" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = config.GetValue<string>("AdminUser:Email") ?? "admin@biblioteca.local";
            var adminPassword = config.GetValue<string>("AdminUser:Password") ?? "Admin123!";

            if (!userManager.Users.Any())
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                var anyAdmin = (await userManager.GetUsersInRoleAsync("Admin")).Any();
                if (!anyAdmin)
                {
                    var firstUser = userManager.Users.FirstOrDefault();
                    if (firstUser != null)
                    {
                        await userManager.AddToRoleAsync(firstUser, "Admin");
                    }
                }
            }
        }
    }
}