using BibliotecaMVC.Data;
using BibliotecaMVC.Repositories;
using BibliotecaMVC.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaDB")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>( options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.AllowedForNewUsers = true;  
})
    .AddEntityFrameworkStores<BibliotecaContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
builder.Services.AddSingleton<IRepositorioLibro, RepositorioenMemoria>();
builder.Services.AddSingleton<IAutorService, AutorService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var usuarios = await userManager.Users.ToListAsync();
    foreach (var usuario in usuarios)
    {
        if ((await userManager.GetRolesAsync(usuario)).Count == 0)
        {
            var result = await userManager.AddToRoleAsync(usuario, "Usuario");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"No se pudo asignar el rol Usuario a {usuario.UserName}: {string.Join(", ", result.Errors.Select(error => error.Description))}"
                );
            }
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    await BibliotecaMVC.Data.SeedData.SeedAsync(scope.ServiceProvider);
}

app.Run();
